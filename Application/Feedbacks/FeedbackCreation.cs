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
            public Command(FeedbackCreationDto feedbackCreationDto)
            {
                FeedbackCreationDto = feedbackCreationDto;
            }
            public FeedbackCreationDto FeedbackCreationDto { get; }
        }

        // ReSharper disable once UnusedType.Global
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

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip with tripId {TripId} doesn't exist",
                            request.FeedbackCreationDto.TripId);
                        return Result<Unit>.NotFound(
                            $"Trip with tripId {request.FeedbackCreationDto.TripId} doesn't exist.");
                    }

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
                        _logger.LogInformation("Trip's feedback is already existed");
                        return Result<Unit>.Failure("Trip's feedback is already existed.");
                    }

                    Feedback newFeedback = new();

                    _mapper.Map(request.FeedbackCreationDto, newFeedback);

                    await _context.Feedback.AddAsync(newFeedback, cancellationToken);

                    try
                    {
                        // Create new transaction to add more point to Biker
                        if (request.FeedbackCreationDto.UserId == trip.KeerId)
                        {
                            var keer = await _context.User
                                .Include(u => u.Wallets.Where(w =>
                                    w.Status == (int) WalletStatus.Current || w.Status == (int) WalletStatus.Old))
                                .Where(u => u.UserId == trip.KeerId)
                                .SingleOrDefaultAsync(cancellationToken);

                            if (keer == null)
                            {
                                _logger.LogInformation("Keer with UserId {UserId} doesn't existed", trip.KeerId);
                                return Result<Unit>.Failure($"Keer with UserId {trip.KeerId} doesn't existed.");
                            }
                            
                            var tripTip = (int) request.FeedbackCreationDto.TripTip!;
                        
                            if (tripTip < 1 || tripTip > keer.TotalPoint)
                            {
                                _logger.LogInformation(
                                    "Tip point should be larger than 1 and smaller than total point of keer");
                                return Result<Unit>.Failure(
                                    "Tip point should be larger than 1 and smaller than total point of keer.");
                            }

                            var oldWallet = keer.Wallets.FirstOrDefault(w => w.Status == (int) WalletStatus.Old);
                            var currentWallet = keer.Wallets.FirstOrDefault(w => w.Status == (int) WalletStatus.Current);
                    
                            if (currentWallet == null)
                            {
                                _logger.LogInformation("Keer with UserId {UserId} doesn't have wallet", trip.KeerId);
                                return Result<Unit>.Failure($"Keer with UserId {trip.KeerId} doesn't have wallet.");
                            }

                            keer.TotalPoint -= tripTip;
                            if (oldWallet != null)
                            {
                                oldWallet.Point -= tripTip;
                        
                                if (oldWallet.Point < 0)
                                {
                                    currentWallet.Point += oldWallet.Point;
                                }
                            }
                            else
                            {
                                currentWallet.Point -= tripTip;
                            }
                            
                            switch (newFeedback.TripStar)
                            {
                                case 4:
                                    await _auto.Run(trip, 5, Constant.TripFeedbackPoint);
                                    await _auto.Run(trip, tripTip, Constant.TripTipPoint);
                                    break;
                                case 5:
                                    await _auto.Run(trip, 10, Constant.TripFeedbackPoint);
                                    await _auto.Run(trip, tripTip, Constant.TripTipPoint);
                                    break;
                            }
                        }
                        
                        _logger.LogInformation("Successfully created feedback");
                        return Result<Unit>.Success(Unit.Value, "Successfully created feedback.",
                            newFeedback.FeedbackId.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Failed to create new feedback");
                        _logger.LogInformation("{Error}", ex.Message);

                        return Result<Unit>.Failure(
                            $"Failed to create new feedback. {ex.Message}");
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