using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications;
using Application.Notifications.DTOs;
using Application.TripTransactions;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripFinishEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int tripId, int bikerId)
            {
                TripId = tripId;
                BikerId = bikerId;
            }
            public int TripId { get; }
            public int BikerId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AutoTripTransactionCreation _auto;
            private readonly NotificationSending _notiSender;
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISchedulerFactory _schedulerFactory;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, ILogger<Handler> logger, ISchedulerFactory schedulerFactory,
                IConfiguration configuration, AutoTripTransactionCreation auto, NotificationSending notiSender)
            {
                _context = context;
                _auto = auto;
                _notiSender = notiSender;
                _logger = logger;
                _schedulerFactory = schedulerFactory;
                _configuration = configuration;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Trip? trip = await _context.Trip
                        .Where(t => t.TripId == request.TripId)
                        .Include(t => t.Route)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.Failure("Trip doesn't exist.");
                    }

                    if (trip.BikerId == null)
                    {
                        _logger.LogInformation("Trip must has Biker before finishing");
                        return Result<Unit>.Failure("Trip must has Biker before finishing.");
                    }

                    if (trip.BikerId != request.BikerId)
                    {
                        _logger.LogInformation("Only Biker of this trip can send request to finish this trip");
                        return Result<Unit>.Failure("Only Biker of this trip can send request to finish this trip.");
                    }

                    switch (trip.Status)
                    {
                        case (int) TripStatus.Matched:
                            _logger.LogInformation("Trip has not started yet");
                            return Result<Unit>.Failure("Trip has not started yet.");
                        case (int) TripStatus.Waiting:
                            _logger.LogInformation("Trip has not started yet");
                            return Result<Unit>.Failure("Trip has not started yet.");
                        case (int) TripStatus.Started:
                            trip.FinishedTime = CurrentTime.GetCurrentTime();
                            trip.Status = (int) TripStatus.Finished;
                            
                            // ReSharper disable StringLiteralTypo
                            var notification = new NotificationDto
                            {
                                NotificationId = Guid.NewGuid(),
                                Title = "Feedback chuyến đi",
                                Content = "Chuyến đi đã kết thúc, mời bạn feedback về chuyến đi",
                                ReceiverId = trip.KeerId,
                                Url = $"{_configuration["ApiPath"]}/{trip.TripId}/feedbacks",
                                IsRead = false,
                                CreatedDate = CurrentTime.GetCurrentTime()
                            };
                            // ReSharper restore StringLiteralTypo

                            await _notiSender.Run(notification);
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
                        if (trip.Status == (int) TripStatus.Finished)
                        {
                            await _auto.Run(trip, trip.Route.DefaultPoint, Constant.TripCompletionPoint);
                            
                            IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

                            string jobName = Constant.GetJobNameAutoCancellation(trip.TripId);
                            string triggerName = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Matched");
                            var triggerKey = new TriggerKey(triggerName, Constant.OneTimeJob);
                
                            var jobTriggerDeletionResult= await scheduler.UnscheduleJob(triggerKey, cancellationToken);

                            if (!jobTriggerDeletionResult) 
                                _logger.LogError("Fail to delete job's trigger with job name {JobName}", jobName);
                
                            _logger.LogInformation("Successfully deleted cancellation job's trigger");
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
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}