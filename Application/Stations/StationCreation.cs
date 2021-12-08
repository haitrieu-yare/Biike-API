using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StationCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public StationCreationDto StationCreationDto { get; init; } = null!;
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

                    Station newStation = new();

                    _mapper.Map(request.StationCreationDto, newStation);

                    var configuration = await _context.Configuration
                        .Where(c => c.ConfigurationName.Equals("ActiveRadius"))
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (configuration == null)
                    {
                        _logger.LogInformation("Failed to create new station because active radius is not found");
                        return Result<Unit>.Failure("Failed to create new station because active radius is not found.");
                    }

                    var activeRadius = Convert.ToDouble(configuration.ConfigurationValue);

                    var centralStation = await _context.Station
                        .Where(s => s.IsCentralPoint == true)
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (centralStation == null)
                    {
                        _logger.LogInformation("Failed to create new station because central station is not found");
                        return Result<Unit>.Failure("Failed to create new station because central station is not found.");
                    }

                    CultureInfo culture = new("en-US");
                    var centralStationCoordinate = centralStation.Coordinate.Split(",");
                    var centralStationLatitude = Convert.ToDouble(centralStationCoordinate[0], culture);
                    var centralStationLongitude = Convert.ToDouble(centralStationCoordinate[1], culture);

                    var newStationCoordinate = newStation.Coordinate.Split(",");
                    var newStationLatitude = Convert.ToDouble(newStationCoordinate[0], culture);
                    var newStationLongitude = Convert.ToDouble(newStationCoordinate[1], culture);

                    var distanceFromCentralPoint = ApplicationUtils.Haversine(centralStationLatitude,
                        centralStationLongitude, newStationLatitude, newStationLongitude);

                    if (distanceFromCentralPoint > activeRadius)
                    {
                        _logger.LogInformation("Failed to create new station because " +
                            "the distance between new station and central station is larger than {Config}", activeRadius);
                        return Result<Unit>.Failure("Failed to create new station because " +
                            $"the distance between new station and central station is larger than {activeRadius}.");
                    }

                    await _context.Station.AddAsync(newStation, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new station");
                        return Result<Unit>.Failure("Failed to create new station.");
                    }

                    _logger.LogInformation("Successfully created new station");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new station.",
                        newStation.StationId.ToString());
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    _logger.LogInformation("Active radius configuration error");
                    return Result<Unit>.Failure("Active radius configuration error.");
                }
                catch (Exception ex) when (ex is OverflowException)
                {
                    _logger.LogInformation("Active radius configuration error");
                    return Result<Unit>.Failure("Active radius configuration error.");
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