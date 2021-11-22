using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
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
    public class TripCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public TripCreationDto TripCreationDto { get; init; } = null!;
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;
            private readonly ISchedulerFactory _schedulerFactory;

            public Handler(DataContext context, IMapper mapper, ISchedulerFactory schedulerFactory,
                ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _schedulerFactory = schedulerFactory;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var limitOneHourTime = CurrentTime.GetCurrentTime().AddHours(1);

                    if (request.TripCreationDto.BookTime!.Value.CompareTo(limitOneHourTime) < 0)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because bookTime {BookTime} " +
                            "is less than {LimitOneHourTime}", request.TripCreationDto.BookTime, limitOneHourTime);
                        return Result<Unit>.Failure("Failed to create new trip schedule because bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is less than {limitOneHourTime}.");
                    }

                    var route = await _context.Route.Where(r => r.DepartureId == request.TripCreationDto.DepartureId)
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
                        .Where(t => t.BookTime == request.TripCreationDto.BookTime)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (tripWithBookTime != null)
                    {
                        _logger.LogInformation(
                            "Failed to create new trip schedule because trip with bookTime {BookTime} " +
                            "is already existed", request.TripCreationDto.BookTime);
                        return Result<Unit>.Failure("Failed to create new trip schedule because trip with bookTime " +
                                                    $"{request.TripCreationDto.BookTime} is already existed.");
                    }

                    var existingTripsCount = await _context.Trip.Where(t => t.KeerId == request.TripCreationDto.KeerId)
                        .Where(t => t.Status == (int) TripStatus.Finding || t.Status == (int) TripStatus.Waiting)
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

                    _logger.LogInformation("Successfully created trip");
                    return Result<Unit>.Success(Unit.Value, "Successfully created trip.", newTrip.TripId.ToString());
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