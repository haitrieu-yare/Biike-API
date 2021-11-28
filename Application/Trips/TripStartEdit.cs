using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripStartEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int tripId, int userId)
            {
                TripId = tripId;
                UserId = userId;
            }

            public int TripId { get; }
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
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

                    Trip trip = await _context.Trip
                        .FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.Failure("Trip doesn't exist.");
                    }

                    if (trip.BikerId == null)
                    {
                        _logger.LogInformation("Trip must has Biker before starting");
                        return Result<Unit>.Failure("Trip must has Biker before starting.");
                    }

                    if (request.UserId != trip.BikerId)
                    {
                        _logger.LogInformation("Biker with UserId {UserId} doesn't belong to this trip", request.UserId);
                        return Result<Unit>.Failure($"Biker with UserId {request.UserId} doesn't belong to this trip.");
                    }

                    switch (trip.Status)
                    {
                        case (int) TripStatus.Matched:
                            _logger.LogInformation("Can not start trip because no one is arrived at waiting point yet");
                            return Result<Unit>.Failure(
                                "Can not start trip because no one is arrived at waiting point yet.");
                        case (int) TripStatus.Waiting:
                            if (trip.SecondPersonArrivalId == null)
                            {
                                var isFirstPersonKeer = trip.FirstPersonArrivalId == trip.KeerId;
                                var personRole = isFirstPersonKeer ? "Biker" : "Keer";
                                    _logger.LogInformation("Can not start trip because {PersonRole} " +
                                                           "has not arrived at waiting point yet", personRole);
                                return Result<Unit>.Failure(
                                    $"Can not start trip because {personRole} has not arrived at waiting point yet.");
                            }
                            else
                            {
                                trip.PickupTime = CurrentTime.GetCurrentTime();
                                trip.Status = (int) TripStatus.Started;
                            }
                            break;
                        case (int) TripStatus.Started:
                            _logger.LogInformation("Trip has already started");
                            return Result<Unit>.Failure("Trip has already started.");
                        case (int) TripStatus.Finished:
                            _logger.LogInformation("Trip has already finished");
                            return Result<Unit>.Failure("Trip has already finished.");
                        case (int) TripStatus.Cancelled:
                            _logger.LogInformation("Trip has already cancelled");
                            return Result<Unit>.Failure("Trip has already cancelled.");
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogInformation("Failed to update trip with TripId {TripId}", request.TripId);
                        return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
                    }

                    _logger.LogInformation("Successfully updated trip with TripId {TripId}", request.TripId);
                    return Result<Unit>.Success(Unit.Value, $"Successfully updated trip with TripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}