using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications;
using Application.Notifications.DTOs;
using Application.Trips.DTOs;
using AutoMapper;
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
    public class TripCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(TripCreationDto tripCreationDto)
            {
                TripCreationDto = tripCreationDto;
            }
            public TripCreationDto TripCreationDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly ISchedulerFactory _schedulerFactory;
            private readonly IConfiguration _configuration;
            private readonly NotificationSending _notiSender;
            private readonly TripCancellationCheck _tripCancellationCheck;

            public Handler(DataContext context, IMapper mapper, ISchedulerFactory schedulerFactory,
                IConfiguration configuration, NotificationSending notiSender, 
                TripCancellationCheck tripCancellationCheck,ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _schedulerFactory = schedulerFactory;
                _configuration = configuration;
                _notiSender = notiSender;
                _tripCancellationCheck = tripCancellationCheck;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (await _tripCancellationCheck.IsLimitExceeded(request.TripCreationDto.KeerId!.Value))
                    {
                        _logger.LogInformation("You can not create new trip because " +
                                               "you have exceeded the maximum number of cancellation in one day");
                        return Result<Unit>.Failure("You can not create new trip because " +
                                                "you have exceeded the maximum number of cancellation in one day.");
                    }

                    var currentTime = CurrentTime.GetCurrentTime();
                    var limitOneHourTime = currentTime.AddHours(1);
                    var limitFifteenMinutesTime = limitOneHourTime.AddMinutes(-45);

                    if (request.TripCreationDto.IsScheduled!.Value &&
                        request.TripCreationDto.BookTime!.Value.CompareTo(limitOneHourTime) < 0)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because bookTime {BookTime} " +
                            "is less than {LimitOneHourTime}", request.TripCreationDto.BookTime, limitOneHourTime);
                        return Result<Unit>.Failure("Failed to create new trip schedule because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is less than {limitOneHourTime}.");
                    }

                    if (!request.TripCreationDto.IsScheduled!.Value &&
                        request.TripCreationDto.BookTime!.Value.CompareTo(limitFifteenMinutesTime) > 0)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip now because bookTime {BookTime} " +
                            "is larger than {LimitFifteenMinutesTime}", request.TripCreationDto.BookTime,
                            limitFifteenMinutesTime);
                        return Result<Unit>.Failure("Failed to create new trip now because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is larger than {limitFifteenMinutesTime}.");
                    }

                    if (!request.TripCreationDto.IsScheduled!.Value &&
                        request.TripCreationDto.BookTime!.Value.CompareTo(currentTime) < 0)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip now because bookTime {BookTime} " +
                            "is earlier than current time {CurrentTime}", request.TripCreationDto.BookTime,
                            currentTime);
                        return Result<Unit>.Failure("Failed to create new trip now because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is earlier than current time {currentTime}.");
                    }

                    var route = await _context.Route
                        .Where(r => r.DepartureId == request.TripCreationDto.DepartureId)
                        .Where(r => r.DestinationId == request.TripCreationDto.DestinationId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (route == null)
                    {
                        _logger.LogInformation(
                            "Route with DepartureId {DepartureId} and DestinationId {DestinationId} doesn't exist",
                            request.TripCreationDto.DepartureId, request.TripCreationDto.DestinationId);
                        return Result<Unit>.NotFound(
                            $"Route with DepartureId {request.TripCreationDto.DepartureId} and " +
                            $"DestinationId {request.TripCreationDto.DestinationId} doesn't exist.");
                    }

                    var tripWithBookTime = await _context.Trip
                        .Where(t => t.KeerId == request.TripCreationDto.KeerId)
                        .Where(t => t.Status != (int) TripStatus.Finished &&
                                    t.Status != (int) TripStatus.Cancelled)
                        .Where(t => t.BookTime == request.TripCreationDto.BookTime)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (tripWithBookTime != null)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip because trip with bookTime {BookTime} " +
                            "is already existed for Keer with KeerId {KeerId}", 
                            request.TripCreationDto.BookTime, request.TripCreationDto.KeerId);
                        return Result<Unit>.Failure("Failed to create new trip because trip with bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is already existed " +
                                                    $"for Keer with KeerId {request.TripCreationDto.KeerId}.");
                    }

                    var existingTripsCount = await _context.Trip
                        .Where(t => t.KeerId == request.TripCreationDto.KeerId)
                        .Where(t => t.Status == (int) TripStatus.Finding || 
                                        t.Status == (int) TripStatus.Matched ||
                                        t.Status == (int) TripStatus.Waiting ||
                                        t.Status == (int) TripStatus.Started)
                        .CountAsync(cancellationToken);

                    if (existingTripsCount + 1 > Constant.MaxTripCount)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because exceeding max number of trips ({MaxTripCount})",
                            Constant.MaxTripCount);
                        return Result<Unit>.Failure(
                            $"Failed to create new trip schedule because exceeding max number of trips ({Constant.MaxTripCount}).");
                    }

                    Trip newTrip = new();

                    _mapper.Map(request.TripCreationDto, newTrip);
                    newTrip.RouteId = route.RouteId;

                    await _context.Trip.AddAsync(newTrip, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new trip");
                        return Result<Unit>.Failure("Failed to create new trip.");
                    }
                    
                    await AutoTripCancellationCreation.Run(_schedulerFactory, newTrip);
                    
                    if (!request.TripCreationDto.IsScheduled.Value)
                    {
                        var blockedBikerIds = await _context.Intimacy
                            .Where(i => i.UserOneId == newTrip.KeerId)
                            .Where(i => i.IsBlock)
                            .Select(i => i.UserTwoId)
                            .ToListAsync(cancellationToken);
                        
                        // ReSharper disable CommentTypo
                        // Mobile s??? g???i time c???ng th??m s???n 15 ph??t, n??n ph???i tr??? 15 ph??t ??i ????? g???i notification
                        // ReSharper restore CommentTypo
                        var timeForKeNow = newTrip.BookTime.AddMinutes(-15);
                        
                        List<int> bikerIds = await _context.BikeAvailability
                            .Include(b => b.User)
                            .Where(b => b.UserId != newTrip.KeerId)
                            .Where(b => !blockedBikerIds.Contains(b.UserId))
                            .Where(b => b.User.IsKeNowAvailable)
                            .Where(b => b.StationId == request.TripCreationDto.DepartureId)
                            .Where(b => b.FromTime.TimeOfDay.CompareTo(timeForKeNow.TimeOfDay) <= 0)
                            .Where(b => b.ToTime.TimeOfDay.CompareTo(timeForKeNow.TimeOfDay) >= 0)
                            .OrderBy(b => b.User.Star)
                            .Select(b => b.UserId)
                            .Distinct().Take(9)
                            .ToListAsync(cancellationToken);

                        if (bikerIds.Count == 0)
                        {
                            newTrip.CancelTime = CurrentTime.GetCurrentTime();
                            // ReSharper disable StringLiteralTypo
                            newTrip.CancelReason = "Kh??ng c?? Biker ph?? h???p ??? th???i ??i???m hi???n t???i";
                            // ReSharper restore StringLiteralTypo
                            newTrip.Status = (int) TripStatus.Cancelled;
                            
                            var cancellationResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                            if (!cancellationResult)
                            {
                                _logger.LogError("There are no bikers available now but the trip is failed to cancel");
                            }
                            
                            _logger.LogInformation("There are no bikers available now");
                            return Result<Unit>.Failure("There are no bikers available now.");
                        }

                        await AutoNotificationSendingCreation.Run(_schedulerFactory, newTrip, bikerIds.Skip(3).ToList());

                        foreach (var bikerId in bikerIds.Take(3))
                        {
                            // ReSharper disable StringLiteralTypo
                            var notification = new NotificationDto
                            {
                                NotificationId = Guid.NewGuid(),
                                Title = Constant.NotificationTitleKeNow,
                                Content = Constant.NotificationContentKeNow,
                                ReceiverId = bikerId,
                                Url = $"{_configuration["ApiPath"]}/trips/{newTrip.TripId}/now",
                                IsRead = false,
                                CreatedDate = CurrentTime.GetCurrentTime()
                            };
                            // ReSharper restore StringLiteralTypo

                            await _notiSender.Run(notification);
                        }
                    }

                    _logger.LogInformation("Successfully created trip");
                    return Result<Unit>.Success(Unit.Value, "Successfully created trip.", newTrip.TripId.ToString());
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