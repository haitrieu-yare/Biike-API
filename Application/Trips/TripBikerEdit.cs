using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications.DTOs;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Firebase.Database;
using Firebase.Database.Query;
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
            public int TripId { get; init; }
            public int BikerId { get; init; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISchedulerFactory _schedulerFactory;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, ISchedulerFactory schedulerFactory, IConfiguration configuration, 
                ILogger<Handler> logger)
            {
                _context = context;
                _schedulerFactory = schedulerFactory;
                _configuration = configuration;
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
                        return Result<Unit>.NotFound("Trip doesn't exist.");
                    }

                    if (oldTrip.KeerId == request.BikerId)
                    {
                        _logger.LogInformation("Biker and Keer can't be the same person");
                        return Result<Unit>.Failure("Biker and Keer can't be the same person.");
                    }

                    User biker = await _context.User.FindAsync(new object[] {request.BikerId}, cancellationToken);

                    if (biker == null)
                    {
                        _logger.LogInformation("Biker doesn't exist");
                        return Result<Unit>.NotFound("Biker doesn't exist.");
                    }

                    if (!biker.IsBikeVerified)
                    {
                        _logger.LogInformation("Biker doesn't have verified bike yet");
                        return Result<Unit>.Failure("Biker doesn't have verified bike yet.");
                    }

                    Bike bike = await _context.Bike.Where(b => b.UserId == biker.UserId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bike == null)
                    {
                        _logger.LogInformation("Bike doesn't exist");
                        return Result<Unit>.NotFound("Bike doesn't exist.");
                    }

                    oldTrip.BikerId = biker.UserId;
                    oldTrip.PlateNumber = bike.PlateNumber;
                    oldTrip.Status = (int) TripStatus.Waiting;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update trip with TripId {request.TripId}", request.TripId);
                        return Result<Unit>.Failure($"Failed to update trip with TripId {request.TripId}.");
                    }

                    IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

                    string jobName = Constant.GetJobNameAutoCancellation(oldTrip.TripId);
                    string triggerName = Constant.GetTriggerNameAutoCancellation(oldTrip.TripId, "Finding");
                    var triggerKey = new TriggerKey(triggerName, Constant.OneTimeJob);
                    
                    var jobTriggerDeletionResult= await scheduler.UnscheduleJob(triggerKey, cancellationToken);
                    
                    _logger.LogInformation("Successfully deleted cancellation job's trigger");

                    if (!jobTriggerDeletionResult) _logger.LogError("Fail to delete job's trigger with job name {JobName}", jobName);
                    
                    var firebaseClient = new FirebaseClient(
                        _configuration["Firebase:RealtimeDatabase"],
                        new FirebaseOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:RealtimeDatabaseSecret"]) 
                        });

                    var options = new JsonSerializerOptions {WriteIndented = true};

                    var notification = new NotificationDto
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Đã Có Biker chấp nhận chuyến đi",
                        Content = $"Biker tên {biker.FullName} đã chấp nhận chuyến đi vào {oldTrip.BookTime} của bạn",
                        ReceiverId = oldTrip.KeerId,
                        Url = $"{_configuration["ApiPath"]}/trips/{oldTrip.TripId}/details",
                        IsRead = false,
                        CreatedDate = CurrentTime.GetCurrentTime()
                    };

                    string notificationJsonString = JsonSerializer.Serialize(notification, options);
                    
                    await firebaseClient
                        .Child("notification")
                        .Child($"{notification.ReceiverId}")
                        .PostAsync(notificationJsonString);

                    _logger.LogInformation("Successfully updated trip with TripId {request.TripId}", request.TripId);
                    return Result<Unit>.Success(Unit.Value, $"Successfully updated trip with TripId {request.TripId}.");
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