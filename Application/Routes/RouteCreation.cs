using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RouteCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public RouteCreationDto RouteCreationDto { get; init; } = null!;
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.RouteCreationDto.DepartureId == request.RouteCreationDto.DestinationId)
                    {
                        _logger.LogInformation("Could not create route because departure station " +
                                               "can not be the same as destination station");
                        return Result<Unit>.Failure("Could not create route because departure station " +
                                                "can not be the same as destination station.");
                    }

                    var departure =
                        await _context.Station.FindAsync(new object[] {request.RouteCreationDto.DepartureId!},
                            cancellationToken);

                    if (departure == null)
                    {
                        _logger.LogInformation("Could not find departure station with StationId {StationId}",
                            request.RouteCreationDto.DepartureId);
                        return Result<Unit>.Failure(
                            $"Could not find departure station with StationId {request.RouteCreationDto.DepartureId}.");
                    }
                    
                    if (departure.IsDeleted)
                    {
                        _logger.LogInformation(
                            "Could not create route because station with stationId {StationId}" + 
                                    "has been deleted", request.RouteCreationDto.DepartureId);
                        return Result<Unit>.Failure(
                            $"Could not create route because station with " +
                            $"StationId {request.RouteCreationDto.DepartureId} has been deleted.");
                    }

                    var destination =
                        await _context.Station.FindAsync(new object[] {request.RouteCreationDto.DestinationId!},
                            cancellationToken);

                    if (destination == null)
                    {
                        _logger.LogInformation("Could not find destination station with StationId {StationId}",
                            request.RouteCreationDto.DestinationId);
                        return Result<Unit>.Failure(
                            $"Could not find destination station with StationId {request.RouteCreationDto.DestinationId}.");
                    }
                    
                    if (destination.IsDeleted)
                    {
                        _logger.LogInformation(
                            "Could not create route because station with stationId {StationId}" + 
                            "has been deleted", request.RouteCreationDto.DestinationId);
                        return Result<Unit>.Failure(
                            $"Could not create route because station with " +
                            $"StationId {request.RouteCreationDto.DestinationId} has been deleted.");
                    }

                    if (departure.AreaId != destination.AreaId)
                    {
                        _logger.LogInformation(
                            "Departure station and destination station does not belong to the same area");
                        return Result<Unit>.Failure(
                            "Departure station and destination station does not belong to the same area.");
                    }

                    if (!departure.IsCentralPoint && !destination.IsCentralPoint)
                    {
                        _logger.LogInformation("Could not create route because none of the station is a central point");
                        return Result<Unit>.Failure("Could not create route because none of the station is a central point.");
                    }

                    Route? oldRoute = await _context.Route
                        .Where(r => r.DepartureId == departure.StationId)
                        .Where(r => r.DestinationId == destination.StationId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (oldRoute != null)
                    {
                        _logger.LogInformation(
                            "Route with departureId {DepartureId} and destinationId {DestinationId} is already existed",
                            request.RouteCreationDto.DepartureId, request.RouteCreationDto.DestinationId);
                        return Result<Unit>.Failure(
                            $"Route with departureId {request.RouteCreationDto.DepartureId} and destinationId " +
                            $"{request.RouteCreationDto.DestinationId} is already existed.");
                    }
                    
                    var newRoute = new Route();
                    _mapper.Map(request.RouteCreationDto, newRoute);

                    if (request.RouteCreationDto.DefaultPoint == 0 ||
                        request.RouteCreationDto.Distance == 0)
                    {
                        CultureInfo culture = new ("en-US");
                        var departureCoordinate = departure.Coordinate.Split(",");
                        double departureLatitude = Convert.ToDouble(departureCoordinate[0], culture);
                        double departureLongitude = Convert.ToDouble(departureCoordinate[1],culture);
                        
                        var destinationCoordinate = destination.Coordinate.Split(",");
                        double destinationLatitude = Convert.ToDouble(destinationCoordinate[0], culture);
                        double destinationLongitude = Convert.ToDouble(destinationCoordinate[1],culture);
                        
                        var distance= ApplicationUtils.Haversine(departureLatitude, departureLongitude,
                            destinationLatitude, destinationLongitude);
                        
                        newRoute.Distance =  Math.Round(distance, 2, MidpointRounding.AwayFromZero);

                        newRoute.DefaultPoint = (int) Math.Round(newRoute.Distance * 2, MidpointRounding.AwayFromZero);
                    }
                    
                    await _context.Route.AddAsync(newRoute, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new route");
                        return Result<Unit>.Failure("Failed to create new route.");
                    }

                    _logger.LogInformation("Successfully created new route");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully created new route.", newRoute.RouteId.ToString());
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