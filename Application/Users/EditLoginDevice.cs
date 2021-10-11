using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class EditLoginDevice
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int UserId { get; init; }
			public UserLoginDeviceDto UserLoginDeviceDto { get; init; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					User user = await _context.User.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null)
					{
						_logger.LogInformation("User doesn't exist");
						return Result<Unit>.NotFound("User doesn't exist.");
					}

					if (user.IsDeleted)
					{
						_logger.LogInformation(
							"User with UserId {request.UserId} has been deleted. " +
							"Please reactivate it if you want to edit it", request.UserId);
						return Result<Unit>.Failure($"User with UserId {request.UserId} has been deleted. " +
						                            "Please reactivate it if you want to edit it.");
					}

					_mapper.Map(request.UserLoginDeviceDto, user);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update user's login device by userId {request.UserId}",
							request.UserId);
						return Result<Unit>.Failure(
							$"Failed to update user's login device by userId {request.UserId}.");
					}

					_logger.LogInformation("Successfully updated user's login device by userId {request.UserId}",
						request.UserId);
					return Result<Unit>.Success(Unit.Value,
						$"Successfully updated user's login device by userId {request.UserId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}