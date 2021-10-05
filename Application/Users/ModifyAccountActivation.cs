using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Enums;
using FirebaseAdmin.Auth;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class ModifyAccountActivation
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int UserId { get; set; }
			public UserActivationDTO UserActivationDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<ModifyAccountActivation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ModifyAccountActivation> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null) return null!;

					if (request.UserActivationDTO.IsEmailVerified != null)
					{
						if (request.UserActivationDTO.IsEmailVerified == false)
						{
							_logger.LogInformation($"Can't set email verification to false.");
							return Result<Unit>.Failure($"Can't set email verification to false.");
						}

						if (user.IsEmailVerified)
						{
							_logger.LogInformation($"User with UserId {request.UserId} has already verified email.");
							return Result<Unit>.Failure(
								$"User with UserId {request.UserId} has already verified email.");
						}

						user.IsEmailVerified = (bool)request.UserActivationDTO.IsEmailVerified;
					}

					if (request.UserActivationDTO.IsPhoneVerified != null)
					{
						if (request.UserActivationDTO.IsPhoneVerified == false)
						{
							_logger.LogInformation($"Can't set phone verification to false.");
							return Result<Unit>.Failure($"Can't set phone verification to false.");
						}

						if (user.IsPhoneVerified)
						{
							_logger.LogInformation($"User with UserId {request.UserId} has already verified phone.");
							return Result<Unit>.Failure(
								$"User with UserId {request.UserId} has already verified phone.");
						}

						user.IsPhoneVerified = (bool)request.UserActivationDTO.IsPhoneVerified;
					}

					if (user.IsEmailVerified && user.IsPhoneVerified)
					{
						user.Status = (int)UserStatus.Active;

						#region Update user on Firebase
						var userToUpdate = new UserRecordArgs()
						{
							Uid = user.UserId.ToString(),
							Disabled = user.Status != (int)UserStatus.Active,
						};
						#endregion

						try
						{
							await FirebaseAuth.DefaultInstance.UpdateUserAsync(userToUpdate, cancellationToken);
						}
						catch (System.Exception ex)
						{
							_logger.LogInformation($"Failed to verify user with UserId {request.UserId} " +
								$"on Firebase. {ex.InnerException?.Message ?? ex.Message}");
							return Result<Unit>.Failure($"Failed to verify user with UserId {request.UserId} " +
								$"on Firebase. {ex.InnerException?.Message ?? ex.Message}");
						}

					}

					var result = await _context.SaveChangesAsync() > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to verify user with UserId {request.UserId}.");
						return Result<Unit>.Failure($"Failed to verify user with UserId {request.UserId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully verified user with UserId {request.UserId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully verified user with UserId {request.UserId}.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
			}
		}
	}
}