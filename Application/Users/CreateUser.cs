using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class CreateUser
	{
		public class Command : IRequest<Result<Unit>>
		{
			public UserCreateDto UserCreateDto { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly Hashing _hashing;
			private readonly ILogger<CreateUser> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<CreateUser> logger, Hashing hashing)
			{
				_context = context;
				_mapper = mapper;
				_hashing = hashing;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var userDb = await _context.User
						.Where(u => u.Email == request.UserCreateDto.Email ||
						            u.PhoneNumber == request.UserCreateDto.PhoneNumber)
						.SingleOrDefaultAsync(cancellationToken);

					if (userDb != null)
					{
						_logger.LogInformation("User with the same email or phone number has already existed.");
						return Result<Unit>.Failure("User with the same email or phone number has already existed.");
					}

					User newUser = new();

					_mapper.Map(request.UserCreateDto, newUser);

					// Setup email, avatar
					newUser.Email = newUser.Email.ToLower();
					string[] fullName = newUser.FullName.Split(" ");
					fullName = fullName.TakeLast(2).ToArray();
					string fullNameString = string.Join("+", fullName);
					newUser.Avatar = $"https://ui-avatars.com/api/?name={fullNameString}" +
					                 "&background=random&rounded=true&size=128";

					// Hash password
					newUser.PasswordHash = _hashing.HashPassword(newUser.PasswordHash);

					#region Create Wallet

					DateTime currentTime = DateTime.UtcNow.AddHours(7);
					DateTime toDate = currentTime;

					if (currentTime.Month >= 1 && currentTime.Month <= 4)
						toDate = DateTime.Parse($"{currentTime.Year}/04/30 23:59:59.9999999");
					else if (currentTime.Month >= 5 && currentTime.Month <= 8)
						toDate = DateTime.Parse($"{currentTime.Year}/08/31 23:59:59.9999999");
					else if (currentTime.Month >= 9 && currentTime.Month <= 12)
						toDate = DateTime.Parse($"{currentTime.Year}/12/31 23:59:59.9999999");

					Wallet newWallet = new()
					{
						User = newUser,
						ToDate = toDate
					};

					#endregion

					using var transaction = _context.Database.BeginTransaction();
					await _context.User.AddAsync(newUser, cancellationToken);
					var resultUser = await _context.SaveChangesAsync(cancellationToken) > 0;
					await _context.Wallet.AddAsync(newWallet, cancellationToken);
					var resultWallet = await _context.SaveChangesAsync(cancellationToken) > 0;

					// Commit transaction if all commands succeed, transaction will auto-rollback
					// when disposed if either commands fails
					transaction.Commit();

					if (!resultUser || !resultWallet)
					{
						_logger.LogInformation("Failed to create new user.");
						return Result<Unit>.Failure("Failed to create new user.");
					}

					try
					{
						#region Create user on Firebase

						var userToCreate = new UserRecordArgs
						{
							Uid = newUser.UserId.ToString(),
							Email = newUser.Email,
							Password = request.UserCreateDto.Password,
							PhoneNumber = newUser.PhoneNumber,
							DisplayName = newUser.FullName,
							PhotoUrl = newUser.Avatar
						};

						await FirebaseAuth.DefaultInstance.CreateUserAsync(userToCreate, cancellationToken);

						#endregion

						#region Import user's role to Firebase

						var claims = new Dictionary<string, object>
						{
							{ "role", (int) RoleStatus.Keer }
						};

						await FirebaseAuth.DefaultInstance
							.SetCustomUserClaimsAsync(userToCreate.Uid, claims, cancellationToken);

						#endregion
					}
					catch (FirebaseAuthException e)
					{
						_logger.LogError("Error create user on Firebase. " +
						                 $"{e.InnerException?.Message ?? e.Message}");
						return Result<Unit>.Failure("Error create user on Firebase. " +
						                            $"{e.InnerException?.Message ?? e.Message}");
					}

					_logger.LogInformation("Successfully created user.");
					return Result<Unit>.Success(
						Unit.Value, "Successfully created user.", newUser.UserId.ToString());
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}