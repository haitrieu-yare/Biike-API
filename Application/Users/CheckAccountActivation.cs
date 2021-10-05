using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class CheckAccountActivation
	{
		public class Query : IRequest<Result<UserActivationDTO>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserActivationDTO>>
		{
			private readonly DataContext _context;
			private readonly ILogger<CheckAccountActivation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CheckAccountActivation> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<UserActivationDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null) return null!;

					UserActivationDTO result = new UserActivationDTO();

					if (user.IsEmailVerified && user.IsPhoneVerified)
					{
						result.IsVerified = true;
					}
					else
					{
						result.IsVerified = false;
					}

					// Set to null to make unnecessary fields excluded from response body.
					result.IsEmailVerified = null;
					result.IsPhoneVerified = null;

					_logger.LogInformation($"Successfully get user activation with UserId {request.UserId}.");
					return Result<UserActivationDTO>.Success(
						result, $"Successfully get user activation with UserId {request.UserId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<UserActivationDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}