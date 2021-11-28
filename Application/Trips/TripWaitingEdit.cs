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
    public class TripWaitingEdit
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
                        _logger.LogInformation("Trip must has Biker before waiting");
                        return Result<Unit>.Failure("Trip must has Biker before waiting.");
                    }

                    if (request.UserId != trip.KeerId && request.UserId != trip.BikerId)
                    {
                        _logger.LogInformation("User with UserId {UserId} doesn't belong to this trip", request.UserId);
                        return Result<Unit>.Failure($"User with UserId {request.UserId} doesn't belong to this trip.");
                    }

                    switch (trip.Status)
                    {
                        case (int) TripStatus.Matching:
                            trip.Status = (int) TripStatus.Waiting;
                            trip.FirstPersonArrivalId = request.UserId;
                            trip.FirstPersonArrivalTime = CurrentTime.GetCurrentTime();
                            break;
                        case (int) TripStatus.Waiting:
                            // Check nếu người thứ nhất đã call api báo đến nơi
                            // thì sẽ không call lại api này được nữa
                            // Đồng thời check nếu người thứ hai đã call api báo đến nơi,
                            // thì cũng sẽ không call lại api này được nữa luôn
                            if (trip.FirstPersonArrivalId == request.UserId ||
                                trip.SecondPersonArrivalId != null && trip.SecondPersonArrivalId == request.UserId)
                            {
                                _logger.LogInformation("User with UserId {UserId} can only request this endpoint once",
                                    request.UserId);
                                return Result<Unit>.Failure(
                                    $"User with UserId {request.UserId} can only request this endpoint once.");
                            }

                            trip.SecondPersonArrivalId = request.UserId;
                            trip.SecondPersonArrivalTime = CurrentTime.GetCurrentTime();
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