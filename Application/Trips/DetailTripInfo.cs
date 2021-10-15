using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DetailTripInfo
    {
        public class Query : IRequest<Result<TripDetailInfoDto>>
        {
            public int TripId { get; init; }
            public int UserRequestId { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<TripDetailInfoDto>>
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

            public async Task<Result<TripDetailInfoDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    User user = await _context.User.FindAsync(new object[] {request.UserRequestId},
                        cancellationToken);

                    if (user == null)
                    {
                        _logger.LogInformation("User who sent request doesn't exist");
                        return Result<TripDetailInfoDto>.NotFound("User who sent request doesn't exist.");
                    }

                    Trip tripDb = await _context.Trip
                        .Where(t => t.TripId == request.TripId)
                        .Include(t => t.Keer)
                        .Include(t => t.Biker)
                        .Include(t => t.Route).ThenInclude(r => r.Departure)
                        .Include(t => t.Route).ThenInclude(r => r.Destination)
                        .Include(t => t.FeedbackList)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (tripDb == null)
                    {
                        _logger.LogInformation("Trip with {TripId} doesn't exist", request.TripId);
                        return Result<TripDetailInfoDto>.NotFound($"Trip with {request.TripId} doesn't exist.");
                    }

                    var isRequestUserInTrip = true;

                    if (tripDb.BikerId == null && request.UserRequestId != tripDb.KeerId)
                        isRequestUserInTrip = false;
                    else if (request.UserRequestId != tripDb.KeerId && request.UserRequestId != tripDb.BikerId)
                        isRequestUserInTrip = false;

                    if (!isRequestUserInTrip)
                    {
                        _logger.LogInformation(
                            "User with UserId {request.UserRequestId} " +
                            "request an unauthorized content of trip with TripId {request.TripId}",
                            request.UserRequestId, request.TripId);
                        return Result<TripDetailInfoDto>.Failure($"User with UserId {request.UserRequestId} " +
                                                                 $"request an unauthorized content of trip with TripId {request.TripId}");
                    }

                    TripDetailInfoDto trip = new();

                    var isKeer = request.UserRequestId == tripDb.KeerId;

                    _mapper.Map(tripDb, trip, o => o.Items["isKeer"] = isKeer);

                    // Set to null to make unnecessary fields excluded from the response body.
                    trip.Feedbacks.ForEach(feedback =>
                    {
                        feedback.TripId = null;
                        feedback.CreatedDate = null;
                    });

                    _logger.LogInformation("Successfully retrieved trip by TripId {request.TripId}", request.TripId);
                    return Result<TripDetailInfoDto>.Success(trip,
                        $"Successfully retrieved trip by TripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<TripDetailInfoDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}