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
			public UserCreateDTO UserCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CreateUser> _logger;
			private readonly Hashing _hashing;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateUser> logger,
				Hashing hashing)
			{
				_hashing = hashing;
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldUser = await _context.User
						.Where(u => u.Email == request.UserCreateDTO.Email ||
							u.PhoneNumber == request.UserCreateDTO.PhoneNumber)
						.SingleOrDefaultAsync(cancellationToken);
					if (oldUser != null)
					{
						_logger.LogInformation("User with the same email or phone number has already existed");
						return Result<Unit>.Failure("User with the same email or phone number has already existed");
					}

					var newUser = new User();
					_mapper.Map(request.UserCreateDTO, newUser);

					#region Setup email, avatar
					newUser.Email = newUser.Email.ToLower();
					var fullName = newUser.FullName.Split(" ");
					fullName = fullName.TakeLast(2).ToArray();
					var fullNameString = string.Join("+", fullName);
					newUser.Avatar = $"https://ui-avatars.com/api/?name={fullNameString}&background=random&rounded=true&size=128";
					#endregion

					#region Hash Password
					newUser.PasswordHash = _hashing.HashPassword(newUser.PasswordHash);
					#endregion

					// FOR DEV ONLY
					newUser.Status = (int)UserStatus.Active;
					//

					await _context.User.AddAsync(newUser, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new user");
						return Result<Unit>.Failure("Failed to create new user");
					}
					else
					{
						_logger.LogInformation("Successfully created user");

						try
						{
							#region Create user on Firebase
							var userToCreate = new UserRecordArgs()
							{
								Uid = newUser.Id.ToString(),
								Email = newUser.Email,
								Password = request.UserCreateDTO.Password,
								PhoneNumber = newUser.PhoneNumber,
								DisplayName = newUser.FullName,
								PhotoUrl = newUser.Avatar,
								Disabled = newUser.Status != (int)UserStatus.Active,
							};
							await FirebaseAuth.DefaultInstance.CreateUserAsync(userToCreate);
							#endregion

							#region Import user's role to Firebase
							var claims = new Dictionary<string, object>()
							{
								{ "admin", true },
							};
							await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userToCreate.Uid, claims);
							#endregion
						}
						catch (FirebaseAuthException e)
						{
							_logger.LogError($"Error create user: {e.Message}");
							return Result<Unit>.Failure($"Error create user: {e.Message}");
						}

						return Result<Unit>.Success(Unit.Value);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}