using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
	public class DeleteBike
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int UserId { get; init; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;

			public Handler(DataContext context, ILogger<Handler> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					User user = await _context.User
						.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null || user.IsDeleted)
					{
						_logger.LogInformation("User to delete bike does not exist");
						return Result<Unit>.NotFound("User to delete bike does not exist.");
					}

					user.IsBikeVerified = false;

					Bike bike = await _context.Bike
						.Where(b => b.UserId == request.UserId)
						.SingleOrDefaultAsync(cancellationToken);

					if (bike == null)
					{
						_logger.LogInformation("Bike does not exist");
						return Result<Unit>.NotFound("Bike does not exist.");
					}

					_context.Bike.Remove(bike);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to delete bike by userId {request.UserId}", request.UserId);
						return Result<Unit>.Failure($"Failed to delete bike by userId {request.UserId}.");
					}

					_logger.LogInformation("Successfully deleted bike by userId {request.UserId}", request.UserId);
					return Result<Unit>.Success(
						Unit.Value, $"Successfully deleted bike by userId {request.UserId}.");
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