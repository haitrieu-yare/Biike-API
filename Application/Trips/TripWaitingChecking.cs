using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    public class TripWaitingChecking
    {
        public class Query : IRequest<Result<bool>>
        {
            public Query(int tripId, int userId)
            {
                TripId = tripId;
                UserId = userId;
            }
            
            public int TripId { get; }
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<bool>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var trip = await _context.Trip
                        .FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<bool>.Failure("Trip doesn't exist");
                    }

                    if (request.UserId != trip.KeerId && request.UserId != trip.BikerId)
                    {
                        _logger.LogInformation("User doesn't belong to this trip");
                        return Result<bool>.Failure("User doesn't belong to this trip.");
                    }

                    var isUserSentWaitingRequest = request.UserId == trip.FirstPersonArrivalId ||
                                                   request.UserId == trip.SecondPersonArrivalId;

                    _logger.LogInformation("Successfully retrieved trip by TripId {request.TripId}", request.TripId);
                    return Result<bool>.Success(isUserSentWaitingRequest,
                        $"Successfully retrieved trip by TripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<bool>.Failure("Request was cancelled.");
                }
            }
        }
    }
}