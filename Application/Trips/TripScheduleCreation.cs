using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripScheduleCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(TripScheduleCreationDto tripScheduleCreationDto)
            {
                TripScheduleCreationDto = tripScheduleCreationDto;
            }
            public TripScheduleCreationDto TripScheduleCreationDto { get; } 
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISchedulerFactory _schedulerFactory;
            private readonly TripCancellationCheck _tripCancellationCheck;

            public Handler(DataContext context, ISchedulerFactory schedulerFactory, 
                TripCancellationCheck tripCancellationCheck ,ILogger<Handler> logger)
            {
                _context = context;
                _schedulerFactory = schedulerFactory;
                _tripCancellationCheck = tripCancellationCheck;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (await _tripCancellationCheck.IsLimitExceeded(request.TripScheduleCreationDto.KeerId!.Value))
                    {
                        _logger.LogInformation("You can not create new trip because " +
                                               "you have exceeded the maximum number of cancellation in one day");
                        return Result<Unit>.Failure("You can not create new trip because " +
                                                    "you have exceeded the maximum number of cancellation in one day.");
                    }

                    if (request.TripScheduleCreationDto.BookTime!.Count == 0)
                    {
                        _logger.LogInformation("Failed to create new trip schedule because list bookTime is empty");
                        return Result<Unit>.Failure(
                            "Failed to create new trip schedule because list bookTime is empty.");
                    }

                    var limitOneHourTime = CurrentTime.GetCurrentTime().AddHours(1);

                    foreach (var bookTime in request.TripScheduleCreationDto.BookTime
                        .Where(bookTime => bookTime.CompareTo(limitOneHourTime) < 0))
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because bookTime {BookTime} " +
                            "is less than {LimitOneHourTime}", bookTime, limitOneHourTime);
                        return Result<Unit>.Failure("Failed to create new trip schedule because bookTime " +
                                                    $"{bookTime} is less than {limitOneHourTime}.");
                    }

                    var route = await _context.Route
                        .Where(r => r.DepartureId == request.TripScheduleCreationDto.DepartureId)
                        .Where(r => r.DestinationId == request.TripScheduleCreationDto.DestinationId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (route == null)
                    {
                        _logger.LogInformation(
                            "Route with DepartureId {DepartureId} and DestinationId {DestinationId} doesn't exist",
                            request.TripScheduleCreationDto.DepartureId, request.TripScheduleCreationDto.DestinationId);
                        return Result<Unit>.NotFound(
                            $"Route with DepartureId {request.TripScheduleCreationDto.DepartureId} and " +
                            $"DestinationId {request.TripScheduleCreationDto.DestinationId} doesn't exist.");
                    }

                    HashSet<DateTime> listBookTime = new();
                    foreach (var bookTime in request.TripScheduleCreationDto.BookTime!)
                    {
                        listBookTime.Add(bookTime);
                    }

                    if (listBookTime.Count != request.TripScheduleCreationDto.BookTime.Count)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because list bookTime has a duplicate bookTime");
                        return Result<Unit>.Failure(
                            "Failed to create new trip schedule because list bookTime has a duplicate bookTime.");
                    }

                    var tripWithBookTime = await _context.Trip
                        .Where(t => t.KeerId == request.TripScheduleCreationDto.KeerId)
                        .Where(t => t.BookTime == request.TripScheduleCreationDto.BookTime!.First())
                        .SingleOrDefaultAsync(cancellationToken);

                    if (tripWithBookTime != null)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because trip with bookTime {BookTime} " +
                            "is already existed", request.TripScheduleCreationDto.BookTime!.First());
                        return Result<Unit>.Failure("Failed to create new trip schedule because trip with bookTime " +
                                                    $"{request.TripScheduleCreationDto.BookTime!.First()} is already existed.");
                    }

                    var existingTripsCount = await _context.Trip
                        .Where(t => t.KeerId == request.TripScheduleCreationDto.KeerId)
                        .Where(t => t.Status == (int) TripStatus.Finding || 
                                        t.Status == (int) TripStatus.Matched ||
                                        t.Status == (int) TripStatus.Waiting ||
                                        t.Status == (int) TripStatus.Started )
                        .CountAsync(cancellationToken);

                    if (existingTripsCount + request.TripScheduleCreationDto.BookTime!.Count > Constant.MaxTripCount)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because exceeding max number of trips ({MaxTripCount})",
                            Constant.MaxTripCount);
                        return Result<Unit>.Failure(
                            $"Failed to create new trip schedule because exceeding max number of trips ({Constant.MaxTripCount}).");
                    }

                    List<Trip> newTrips = request.TripScheduleCreationDto.BookTime!.Select(bookTime =>
                            new Trip
                            {
                                RouteId = route.RouteId,
                                KeerId = (int) request.TripScheduleCreationDto.KeerId!,
                                BookTime = bookTime,
                                IsScheduled = true
                            })
                        .ToList();

                    await _context.Trip.AddRangeAsync(newTrips, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create multiple trips by schedule");
                        return Result<Unit>.Failure("Failed to create multiple trips by schedule.");
                    }

                    foreach (var newTrip in newTrips)
                    {
                        await AutoTripCancellationCreation.Run(_schedulerFactory, newTrip);
                    }

                    _logger.LogInformation("Successfully created multiple trips by schedule");
                    return Result<Unit>.Success(Unit.Value, "Successfully created multiple trips by schedule.",
                        newTrips.First().TripId.ToString());
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