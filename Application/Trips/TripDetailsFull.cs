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
    public class TripDetailsFull
    {
        public class Query : IRequest<Result<TripDetailsFullDto>>
        {
            public int TripId { get; init; }
            public int UserRequestId { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<TripDetailsFullDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly TripCancellationCheck _tripCancellationCheck;

            public Handler(DataContext context, IMapper mapper, 
                TripCancellationCheck tripCancellationCheck,ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _tripCancellationCheck = tripCancellationCheck;
                _logger = logger;
            }

            public async Task<Result<TripDetailsFullDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    User user = await _context.User.FindAsync(new object[] {request.UserRequestId},
                        cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User who sent request doesn't exist");
                        return Result<TripDetailsFullDto>.NotFound("User who sent request doesn't exist.");
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
                        return Result<TripDetailsFullDto>.NotFound($"Trip with {request.TripId} doesn't exist.");
                    }

                    // var isRequestUserInTrip = true;
                    //
                    // if (tripDb.BikerId == null && request.UserRequestId != tripDb.KeerId)
                    //     isRequestUserInTrip = false;
                    // else if (request.UserRequestId != tripDb.KeerId && request.UserRequestId != tripDb.BikerId)
                    //     isRequestUserInTrip = false;
                    //
                    // if (!isRequestUserInTrip)
                    // {
                    //     _logger.LogInformation(
                    //         "User with UserId {request.UserRequestId} " +
                    //         "request an unauthorized content of trip with TripId {request.TripId}",
                    //         request.UserRequestId, request.TripId);
                    //     return Result<TripDetailsFullDto>.Failure($"User with UserId {request.UserRequestId} " +
                    //                                              $"request an unauthorized content of trip with TripId {request.TripId}");
                    // }

                    TripDetailsFullDto trip = new();

                    var isKeer = request.UserRequestId == tripDb.KeerId;

                    _mapper.Map(tripDb, trip, o => o.Items["isKeer"] = isKeer);

                    // Set to null to make unnecessary fields excluded from the response body.
                    trip.Feedbacks.ForEach(feedback =>
                    {
                        feedback.TripId = null;
                        feedback.CreatedDate = null;
                    });

                    if (tripDb.BikerId != null)
                    {
                        var bike = await _context.Bike
                            .Where(b => b.UserId == tripDb.BikerId)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (bike == null)
                        {
                            _logger.LogInformation("Biker does not have bike");
                            return Result<TripDetailsFullDto>.Failure($"Biker does not have bike.");
                        }

                        trip.Brand = bike.Brand;
                        trip.Color = bike.Color;
                        trip.PlateNumber = bike.PlateNumber;
                    }

                    trip.IsCancellationLimitExceeded = await _tripCancellationCheck.IsLimitExceeded(user.UserId);
                    
                    _logger.LogInformation("Successfully retrieved trip by TripId {request.TripId}", request.TripId);
                    return Result<TripDetailsFullDto>.Success(trip,
                        $"Successfully retrieved trip by TripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<TripDetailsFullDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}