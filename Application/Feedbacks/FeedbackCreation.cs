using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Feedbacks.DTOs;
using Application.TripTransactions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Feedbacks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedbackCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public FeedbackCreationDto FeedbackCreationDto { get; init; } = null!;
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AutoTripTransactionCreation _auto;
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger,
                AutoTripTransactionCreation auto)
            {
                _context = context;
                _mapper = mapper;
                _auto = auto;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Trip trip = await _context.Trip.FindAsync(new object[] {request.FeedbackCreationDto.TripId!},
                        cancellationToken);

                    if (trip == null) return Result<Unit>.NotFound("Trip doesn't exist.");

                    if (request.FeedbackCreationDto.UserId != trip.KeerId &&
                        request.FeedbackCreationDto.UserId != trip.BikerId)
                    {
                        _logger.LogInformation("User send feedback must be in the trip with tripId {trip.TripId}",
                            trip.TripId);
                        return Result<Unit>.Failure(
                            $"User send feedback must be in the trip with tripId {trip.TripId}.");
                    }

                    if (trip.Status == (int) TripStatus.Cancelled)
                    {
                        _logger.LogInformation("Trip has already been cancelled");
                        return Result<Unit>.Failure("Trip has already been cancelled.");
                    }

                    if (trip.Status != (int) TripStatus.Finished)
                    {
                        _logger.LogInformation("Can't create feedback because trip hasn't finished yet");
                        return Result<Unit>.Failure("Can't create feedback because trip hasn't finished yet.");
                    }

                    List<Feedback> feedbacks = await _context.Feedback
                        .Where(f => f.TripId == request.FeedbackCreationDto.TripId)
                        .ToListAsync(cancellationToken);

                    var existedFeedback = feedbacks.Find(f => f.UserId == request.FeedbackCreationDto.UserId);

                    if (existedFeedback != null)
                    {
                        _logger.LogInformation("Trip feedback is already existed");
                        return Result<Unit>.Failure("Trip feedback is already existed.");
                    }

                    Feedback newFeedback = new();

                    _mapper.Map(request.FeedbackCreationDto, newFeedback);

                    await _context.Feedback.AddAsync(newFeedback, cancellationToken);

                    try
                    {
                        // Create new transaction to add more point to Biker
                        if (request.FeedbackCreationDto.UserId == trip.KeerId)
                            switch (newFeedback.TripStar)
                            {
                                case 4:
                                    await _auto.Run(trip, 5, cancellationToken);
                                    break;
                                case 5:
                                    await _auto.Run(trip, 10, cancellationToken);
                                    break;
                            }

                        _logger.LogInformation("Successfully created feedback");
                        return Result<Unit>.Success(Unit.Value, "Successfully created feedback.",
                            newFeedback.FeedbackId.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Failed to create new feedback");
                        _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);

                        return Result<Unit>.Failure(
                            $"Failed to create new feedback. {ex.InnerException?.Message ?? ex.Message}");
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