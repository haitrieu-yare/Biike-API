using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips.DTOs
{
	public class EditTripProcess
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int TripId { get; set; }
			public DateTime? Time { get; set; }
		}
		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<EditTripProcess> _logger;
			private readonly AutoCreateTripTransaction _autoCreate;
			public Handler(DataContext context, ILogger<EditTripProcess> logger,
				AutoCreateTripTransaction autoCreate)
			{
				_logger = logger;
				_context = context;
				_autoCreate = autoCreate;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Time == null)
					{
						_logger.LogInformation("No parameters are received");
						return Result<Unit>.Failure("No parameters are received");
					}

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);
					if (oldTrip == null) return null!;

					if (oldTrip.BikerId == null)
					{
						_logger.LogInformation("Trip must has Biker before starting");
						return Result<Unit>.Failure("Trip must has Biker before starting");
					}

					switch (oldTrip.Status)
					{
						case (int)TripStatus.Waiting:
							oldTrip.PickupTime = request.Time;
							oldTrip.Status = (int)TripStatus.Started;
							break;
						case (int)TripStatus.Started:
							oldTrip.FinishedTime = request.Time;
							oldTrip.Status = (int)TripStatus.Finished;
							break;
						case (int)TripStatus.Finished:
							_logger.LogInformation("Trip has already finished");
							return Result<Unit>.Failure("Trip has already finished");
						case (int)TripStatus.Cancelled:
							_logger.LogInformation("Trip has already cancelled");
							return Result<Unit>.Failure("Trip has already cancelled");
					}

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update trip");
						return Result<Unit>.Failure("Failed to update trip");
					}
					else
					{
						_logger.LogInformation("Successfully updated trip");

						if (oldTrip.Status == (int)TripStatus.Finished)
							await _autoCreate.Run(oldTrip, 10, cancellationToken);

						return Result<Unit>.Success(Unit.Value, "Successfully updated trip");
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