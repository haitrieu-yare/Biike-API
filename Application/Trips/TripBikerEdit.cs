using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications;
using Application.Notifications.DTOs;
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
    public class TripBikerEdit
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
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISchedulerFactory _schedulerFactory;
            private readonly IConfiguration _configuration;
            private readonly NotificationSending _notiSender;

            public Handler(DataContext context, ISchedulerFactory schedulerFactory, IConfiguration configuration, 
                NotificationSending notiSender, ILogger<Handler> logger)
            {
                _context = context;
                _schedulerFactory = schedulerFactory;
                _configuration = configuration;
                _notiSender = notiSender;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Trip trip = await _context.Trip.FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.Failure("Trip doesn't exist.");
                    }

                    if (trip.KeerId == request.BikerId)
                    {
                        _logger.LogInformation("Biker and Keer can't be the same person");
                        return Result<Unit>.Failure("Biker and Keer can't be the same person.");
                    }

                    User biker = await _context.User
                        .Where(u => u.UserId == request.BikerId)
                        .Where(u => u.IsDeleted == false)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (biker == null)
                    {
                        _logger.LogInformation("Biker doesn't exist");
                        return Result<Unit>.Failure("Biker doesn't exist.");
                    }

                    if (!biker.IsBikeVerified)
                    {
                        _logger.LogInformation("Biker doesn't have verified bike yet");
                        return Result<Unit>.Failure("Biker doesn't have verified bike yet.");
                    }

                    Bike bike = await _context.Bike
                        .Where(b => b.UserId == biker.UserId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bike == null)
                    {
                        _logger.LogInformation("Bike doesn't exist");
                        return Result<Unit>.Failure("Bike doesn't exist.");
                    }

                    trip.BikerId = biker.UserId;
                    trip.PlateNumber = bike.PlateNumber;
                    trip.Status = (int) TripStatus.Matched;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update trip with TripId {TripId}", request.TripId);
                        return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
                    }
                    
                    IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

                    string jobName = Constant.GetJobNameAutoCancellation(trip.TripId);
                    string triggerName = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Finding");
                    var triggerKey = new TriggerKey(triggerName, Constant.OneTimeJob);
                
                    var jobTriggerDeletionResult= await scheduler.UnscheduleJob(triggerKey, cancellationToken);

                    if (!jobTriggerDeletionResult) 
                        _logger.LogError("Fail to delete job's trigger with job name {JobName}", jobName);
                
                    _logger.LogInformation("Successfully deleted cancellation job's trigger");
                    
                    // ReSharper disable StringLiteralTypo
                    var notification = new NotificationDto
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Đã Có Biker chấp nhận chuyến đi",
                        Content = $"Biker tên {biker.FullName} đã chấp nhận chuyến đi vào {trip.BookTime} của bạn",
                        ReceiverId = trip.KeerId,
                        Url = $"{_configuration["ApiPath"]}/trips/{trip.TripId}/details",
                        IsRead = false,
                        CreatedDate = CurrentTime.GetCurrentTime()
                    };
                    // ReSharper restore StringLiteralTypo

                    await _notiSender.Run(notification);

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