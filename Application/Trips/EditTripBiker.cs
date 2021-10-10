using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class EditTripBiker
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int TripId { get; set; }
			public int BikerId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<EditTripBiker> _logger;

			public Handler(DataContext context, ILogger<EditTripBiker> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);

					if (oldTrip == null) return Result<Unit>.NotFound("Trip doesn't exist.");

					if (oldTrip.KeerId == request.BikerId)
					{
						_logger.LogInformation("Biker and Keer can't be the same person.");
						return Result<Unit>.Failure("Biker and Keer can't be the same person.");
					}

					var biker = await _context.User
						.FindAsync(new object[] { request.BikerId }, cancellationToken);

					if (!biker.IsBikeVerified)
					{
						_logger.LogInformation("Biker doesn't have verified bike yet.");
						return Result<Unit>.Failure("Biker doesn't have verified bike yet.");
					}

					var bike = await _context.Bike
						.Where(b => b.UserId == biker.UserId)
						.SingleOrDefaultAsync(cancellationToken);

					oldTrip.BikerId = biker.UserId;
					oldTrip.PlateNumber = bike.PlateNumber;
					oldTrip.Status = (int) TripStatus.Waiting;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to update trip with TripId {request.TripId}.");
						return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
					}

					_logger.LogInformation($"Successfully updated trip with TripId {request.TripId}.");
					return Result<Unit>.Success(
						Unit.Value, $"Successfully updated trip with TripId {request.TripId}.");
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