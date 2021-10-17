using System;
using System.Collections.Generic;
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
    public class StationListByStationId
    {
        public class Query : IRequest<Result<List<StationDto>>>
        {
            public bool IsAdmin { get; init; }
            public int DepartureId { get; init; }
            public int DestinationId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<StationDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<List<StationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.DepartureId <= 0 && request.DestinationId == 0)
                    {
                        _logger.LogInformation("DepartureId must be larger than 0");
                        return Result<List<StationDto>>.Failure("DepartureId must be larger than 0.");
                    }

                    if (request.DestinationId <= 0 && request.DepartureId == 0)
                    {
                        _logger.LogInformation("DestinationId must be larger than 0");
                        return Result<List<StationDto>>.Failure("DestinationId must be larger than 0.");
                    }

                    if (request.DestinationId <= 0 && request.DepartureId <= 0)
                    {
                        _logger.LogInformation("DepartureId or DestinationId must be provided");
                        return Result<List<StationDto>>.Failure("DepartureId or DestinationId must be provided.");
                    }

                    var option = true;

                    switch (request.DepartureId)
                    {
                        case > 0 when request.DestinationId > 0:
                            _logger.LogInformation("Only DepartureId or DestinationId can be provided at a time" +
                                                   "Do not send both parameters");
                            return Result<List<StationDto>>.Failure(
                                "Only DepartureId or DestinationId can be provided at a time." +
                                "Do not send both parameters");
                        case > 0:
                            option = true;
                            break;
                    }

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<StationDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<StationDto>>.Failure("Limit must be larger than 0.");
                    }

                    int totalRecord;

                    if (request.IsAdmin)
                    {
                        totalRecord = option switch
                        {
                            true => await _context.Route.Where(r => r.DepartureId == request.DepartureId)
                                .CountAsync(cancellationToken),
                            false => await _context.Route.Where(r => r.DestinationId == request.DestinationId)
                                .CountAsync(cancellationToken)
                        };
                    }
                    else
                    {
                        totalRecord = option switch
                        {
                            true => await _context.Route.Where(s => s.IsDeleted != true)
                                .Where(r => r.DepartureId == request.DepartureId)
                                .CountAsync(cancellationToken),
                            false => await _context.Route.Where(s => s.IsDeleted != true)
                                .Where(r => r.DestinationId == request.DestinationId)
                                .CountAsync(cancellationToken)
                        };
                    }

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);
                    List<Station> stationsDb = new();
                    List<StationDto> stations = new();

                    if (request.Page <= lastPage)
                    {
                        List<Route> routes;
                        if (request.IsAdmin)
                        {
                            routes = await _context.Route
                                .Where(r => option
                                    ? r.DepartureId == request.DepartureId
                                    : r.DestinationId == request.DestinationId)
                                .Include(r => option ? r.Destination : r.Departure)
                                .OrderBy(r => r.DestinationId)
                                .Skip((request.Page - 1) * request.Limit)
                                .Take(request.Limit)
                                .ToListAsync(cancellationToken);
                        }
                        else
                        {
                            routes = await _context.Route.Where(s => s.IsDeleted != true)
                                .Where(r => option
                                    ? r.DepartureId == request.DepartureId
                                    : r.DestinationId == request.DestinationId)
                                .Include(r => option ? r.Destination : r.Departure)
                                .OrderBy(r => r.DestinationId)
                                .Skip((request.Page - 1) * request.Limit)
                                .Take(request.Limit)
                                .ToListAsync(cancellationToken);
                        }

                        stationsDb.AddRange(routes.Select(route => option ? route.Destination : route.Departure));

                        _mapper.Map(stationsDb, stations);

                        if (!request.IsAdmin)
                        {
                            // Set to null to make unnecessary fields excluded from the response body.
                            stations.ForEach(s =>
                            {
                                s.CreatedDate = null;
                                s.IsDeleted = null;
                            });
                        }
                    }

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, stations.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all stations");
                    return Result<List<StationDto>>.Success(stations, "Successfully retrieved list of all stations.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<StationDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}