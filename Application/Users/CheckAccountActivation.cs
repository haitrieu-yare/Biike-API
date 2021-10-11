using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class CheckAccountActivation
	{
		public class Query : IRequest<Result<UserActivationDto>>
		{
			public int UserId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<UserActivationDto>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;

			public Handler(DataContext context, ILogger<Handler> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<UserActivationDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					User user = await _context.User.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null)
					{
						_logger.LogInformation("User doesn't exist");
						return Result<UserActivationDto>.NotFound("User doesn't exist.");
					}

					UserActivationDto result = new()
					{
						IsVerified = user.Status == (int) UserStatus.Active,
						// Set to null to make unnecessary fields excluded from the response body.
						IsEmailVerified = null,
						IsPhoneVerified = null
					};

					_logger.LogInformation("Successfully get user activation with UserId {request.UserId}",
						request.UserId);
					return Result<UserActivationDto>.Success(result,
						$"Successfully get user activation with UserId {request.UserId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<UserActivationDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}