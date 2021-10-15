using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EditTripProcess
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int TripId { get; init; }
            public int BikerId { get; init; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AutoCreateTripTransaction _autoCreate;
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger, AutoCreateTripTransaction autoCreate)
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

                    Trip oldTrip = await _context.Trip.FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (oldTrip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.Failure("Trip doesn't exist.");
                    }

                    if (oldTrip.BikerId == null)
                    {
                        _logger.LogInformation("Trip must has Biker before starting");
                        return Result<Unit>.Failure("Trip must has Biker before starting.");
                    }

                    if (oldTrip.KeerId == request.BikerId)
                    {
                        _logger.LogInformation("Biker and Keer can't be the same person");
                        return Result<Unit>.Failure("Biker and Keer can't be the same person.");
                    }

                    if (oldTrip.BikerId != request.BikerId)
                    {
                        _logger.LogInformation("BikerId of trip doesn't match bikerId in request");
                        return Result<Unit>.Failure("BikerId of trip doesn't match bikerId in request.");
                    }

                    switch (oldTrip.Status)
                    {
                        case (int) TripStatus.Waiting:
                            oldTrip.PickupTime = CurrentTime.GetCurrentTime();
                            oldTrip.Status = (int) TripStatus.Started;
                            break;
                        case (int) TripStatus.Started:
                            oldTrip.FinishedTime = CurrentTime.GetCurrentTime();
                            oldTrip.Status = (int) TripStatus.Finished;
                            break;
                        case (int) TripStatus.Finished:
                            _logger.LogInformation("Trip has already finished");
                            return Result<Unit>.Failure("Trip has already finished.");
                        case (int) TripStatus.Cancelled:
                            _logger.LogInformation("Trip has already cancelled");
                            return Result<Unit>.Failure("Trip has already cancelled.");
                    }

                    try
                    {
                        // TODO: Point based on Kilometer
                        if (oldTrip.Status == (int) TripStatus.Finished)
                        {
                            await _autoCreate.Run(oldTrip, 10, cancellationToken);
                        }
                        else
                        {
                            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                            if (!result)
                            {
                                _logger.LogInformation("Failed to update trip with TripId {request.TripId}",
                                    request.TripId);
                                return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
                            }
                        }

                        _logger.LogInformation("Successfully updated trip with TripId {request.TripId}",
                            request.TripId);
                        return Result<Unit>.Success(Unit.Value,
                            $"Successfully updated trip with TripId {request.TripId}.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Failed to update trip with TripId {request.TripId}. {Error}",
                            request.TripId, ex.InnerException?.Message ?? ex.Message);
                        return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}. " +
                                                    (ex.InnerException?.Message ?? ex.Message));
                    }
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