using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.TripTransactions;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Trips.DTOs
{
	public class EditTripProcess
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int TripId { get; set; }
			public int BikerId { get; set; }
			public DateTime? Time { get; set; }
		}
		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly AutoCreateTripTransaction _autoCreate;
			public Handler(DataContext context, ILogger<Handler> logger,
				AutoCreateTripTransaction autoCreate)
			{
				_context = context;
				_autoCreate = autoCreate;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Time == null)
					{
						_logger.LogInformation("No parameters Time received");
						return Result<Unit>.Failure("No parameters Time received.");
					}

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);

					if (oldTrip == null) return null!;

					if (oldTrip.BikerId == null)
					{
						_logger.LogInformation("Trip must has Biker before starting");
						return Result<Unit>.Failure("Trip must has Biker before starting.");
					}
					else if (oldTrip.KeerId == request.BikerId)
					{
						_logger.LogInformation("Biker and Keer can't be the same person");
						return Result<Unit>.Failure("Biker and Keer can't be the same person.");
					}
					else if (oldTrip.BikerId != request.BikerId)
					{
						_logger.LogInformation("BikerId of trip doesn't match bikerId in request");
						return Result<Unit>.Failure("BikerId of trip doesn't match bikerId in request.");
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
							return Result<Unit>.Failure("Trip has already finished.");
						case (int)TripStatus.Cancelled:
							_logger.LogInformation("Trip has already cancelled");
							return Result<Unit>.Failure("Trip has already cancelled.");
					}

					try
					{
						bool result = true;
						if (oldTrip.Status == (int)TripStatus.Finished)
						{
							await _autoCreate.Run(oldTrip, 10, cancellationToken);
						}
						else
						{
							result = await _context.SaveChangesAsync() > 0;
							if (!result)
							{
								_logger.LogInformation($"Failed to update trip with TripId {request.TripId}");
								return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
							}
						}

						_logger.LogInformation($"Successfully updated trip with TripId {request.TripId}");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully updated trip with TripId {request.TripId}.");
					}
					catch (System.Exception ex)
					{
						_logger.LogInformation($"Failed to update trip with TripId {request.TripId}" +
							ex.InnerException?.Message ?? ex.Message);
						return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}. " +
							ex.InnerException?.Message ?? ex.Message);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}