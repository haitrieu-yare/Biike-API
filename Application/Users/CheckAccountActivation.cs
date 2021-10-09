using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
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
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserActivationDto>>
		{
			private readonly DataContext _context;
			private readonly ILogger<CheckAccountActivation> _logger;

			public Handler(DataContext context, IMapper mapper, ILogger<CheckAccountActivation> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<UserActivationDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null) return null!;

					UserActivationDto result = new();

					if (user.Status == (int) UserStatus.Active)
						result.IsVerified = true;
					else
						result.IsVerified = false;

					// Set to null to make unnecessary fields excluded from response body.
					result.IsEmailVerified = null;
					result.IsPhoneVerified = null;

					_logger.LogInformation($"Successfully get user activation with UserId {request.UserId}.");
					return Result<UserActivationDto>.Success(
						result, $"Successfully get user activation with UserId {request.UserId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<UserActivationDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}