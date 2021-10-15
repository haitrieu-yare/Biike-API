using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Feedbacks.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Feedbacks
{
    public class ListFeedbacksByTrip
    {
        public class Query : IRequest<Result<List<FeedbackDto>>>
        {
            public int TripId { get; init; }
            public int UserRequestId { get; init; }
            public bool IsAdmin { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<FeedbackDto>>>
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

            public async Task<Result<List<FeedbackDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Trip trip = await _context.Trip.FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (trip == null) return Result<List<FeedbackDto>>.NotFound("Trip doesn't exist.");

                    var isRequestUserInTrip = true;

                    if (trip.BikerId == null && request.UserRequestId != trip.KeerId && !request.IsAdmin)
                        isRequestUserInTrip = false;
                    else if (trip.BikerId != null && request.UserRequestId != trip.BikerId &&
                             request.UserRequestId != trip.KeerId && !request.IsAdmin)
                        isRequestUserInTrip = false;

                    if (!isRequestUserInTrip)
                    {
                        _logger.LogInformation("User send request must be in the trip with tripId {trip.TripId}",
                            trip.TripId);
                        return Result<List<FeedbackDto>>.Failure(
                            $"User send request must be in the trip with tripId {trip.TripId}.");
                    }

                    if (trip.Status != (int) TripStatus.Finished)
                    {
                        _logger.LogInformation("Trip with tripId {trip.TripId} hasn't finished", trip.TripId);
                        return Result<List<FeedbackDto>>.Failure($"Trip with tripId {trip.TripId} hasn't finished.");
                    }

                    List<FeedbackDto> feedbacks = await _context.Feedback.Where(s => s.TripId == request.TripId)
                        .ProjectTo<FeedbackDto>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);

                    if (!request.IsAdmin)
                        // Set to null to make unnecessary fields excluded from the response body.
                        feedbacks.ForEach(f => f.CreatedDate = null);

                    _logger.LogInformation("Successfully retrieved trip's feedbacks by tripId {request.TripId}",
                        request.TripId);
                    return Result<List<FeedbackDto>>.Success(feedbacks,
                        $"Successfully retrieved trip's feedbacks by tripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<FeedbackDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}