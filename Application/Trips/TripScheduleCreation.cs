using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
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
            public TripScheduleCreationDto TripScheduleCreationDto { get; init; } = null!;
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISchedulerFactory _schedulerFactory;

            public Handler(DataContext context, ISchedulerFactory schedulerFactory,
                ILogger<Handler> logger)
            {
                _context = context;
                _schedulerFactory = schedulerFactory;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

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
                        .Where(t => t.Status == (int) TripStatus.Finding || t.Status == (int) TripStatus.Waiting)
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
                                IsScheduled = (bool) request.TripScheduleCreationDto.IsScheduled!
                            })
                        .ToList();

                    await _context.Trip.AddRangeAsync(newTrips, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new trip schedule");
                        return Result<Unit>.Failure("Failed to create new trip schedule.");
                    }

                    foreach (var newTrip in newTrips)
                    {
                        await AutoTripCancellationCreation.Run(_schedulerFactory, newTrip);
                    }

                    _logger.LogInformation("Successfully created multiple trips");
                    return Result<Unit>.Success(Unit.Value, "Successfully created multiple trips.");
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