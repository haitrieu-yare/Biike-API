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

            public Handler(DataContext context, IMapper mapper, ISchedulerFactory schedulerFactory,
                IConfiguration configuration, NotificationSending notiSender, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
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

                    var limitOneHourTime = CurrentTime.GetCurrentTime().AddHours(1);
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
                        .Where(t => t.BookTime == request.TripCreationDto.BookTime)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (tripWithBookTime != null)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip because trip with bookTime {BookTime} " +
                            "is already existed for Keer with KeerId {KeerId}", 
                            request.TripCreationDto.BookTime, request.TripCreationDto.KeerId);
                        return Result<Unit>.Failure("Failed to create new trip because trip with bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is already existed" +
                                                    $"for Keer with KeerId {request.TripCreationDto.KeerId}.");
                    }

                    var existingTripsCount = await _context.Trip
                        .Where(t => t.KeerId == request.TripCreationDto.KeerId)
                        .Where(t => t.Status == (int) TripStatus.Finding || 
                                        t.Status == (int) TripStatus.Matching ||
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

                    if (request.TripCreationDto.IsScheduled.Value)
                    {
                        await AutoTripCancellationCreation.Run(_schedulerFactory, newTrip);
                    }
                    else
                    {
                        List<User> bikers = await _context.User
                            .Where(u => u.IsBikeVerified == true)
                            .OrderBy(u => u.UserId)
                            .Take(3)
                            .ToListAsync(cancellationToken);

                        foreach (var biker in bikers)
                        {
                            var notification = new NotificationDto
                            {
                                NotificationId = Guid.NewGuid(),
                                Title = Constant.NotificationTitleKeNow,
                                Content = Constant.NotificationContentKeNow,
                                ReceiverId = biker.UserId,
                                Url = $"{_configuration["ApiPath"]}/trips/{newTrip.TripId}/details",
                                IsRead = false,
                                CreatedDate = CurrentTime.GetCurrentTime()
                            };

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