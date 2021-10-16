using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NewlyCreatedTripList
    {
        public class Query : IRequest<Result<List<TripDto>>>
        {
            public int UserId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
            public int DepartureId { get; init; }
            public int DestinationId { get; init; }
            public string? DateTime { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TripDto>>>
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

            public async Task<Result<List<TripDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(request.DateTime))
                    {
                        _logger.LogInformation("DateTime must be provided");
                        return Result<List<TripDto>>.Failure("DateTime must be provided.");
                    }

                    if (request.DepartureId <= 0)
                    {
                        _logger.LogInformation("DepartureId must be larger than 0");
                        return Result<List<TripDto>>.Failure("DepartureId must be larger than 0.");
                    }
                    
                    if (request.DestinationId <= 0)
                    {
                        _logger.LogInformation("DestinationId must be larger than 0");
                        return Result<List<TripDto>>.Failure("DestinationId must be larger than 0.");
                    }

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<TripDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<TripDto>>.Failure("Limit must be larger than 0.");
                    }

                    if (!DateTime.TryParse(request.DateTime, out var dateTime))
                    {
                        _logger.LogInformation("DateTime parameter format is invalid");
                        return Result<List<TripDto>>.Failure("DateTime parameter format is invalid.");
                    }

                    var currentTime = CurrentTime.GetCurrentTime();
                    
                    if (dateTime.CompareTo(currentTime) < 0)
                    {
                        _logger.LogInformation("DateTime parameter value must be later than current time {currentTime}",
                            currentTime);
                        return Result<List<TripDto>>.Failure(
                            $"DateTime parameter value must be later than current time {currentTime}.");
                    }
                    
                    if (dateTime.TimeOfDay.CompareTo(new TimeSpan(5,0,0)) < 0 ||
                        dateTime.TimeOfDay.CompareTo(new TimeSpan(21,0,0)) > 0)
                    {
                        _logger.LogInformation("DateTime parameter value must be later than 5AM and before 21PM");
                        return Result<List<TripDto>>.Failure(
                            "DateTime parameter value must be later than 5AM and before 21PM.");
                    }

                    var totalRecord = await _context.Trip
                        .Where(t => t.KeerId != request.UserId)
                        .Where(t => t.Status == (int) TripStatus.Finding)
                        .Where(t => t.BookTime >= dateTime.AddMinutes(-15) && t.BookTime <= dateTime.AddMinutes(15))
                        .Where(t => t.Route.DepartureId == request.DepartureId)
                        .Where(t => t.Route.DestinationId == request.DestinationId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<TripDto> trips = new();

                    if (request.Page <= lastPage)
                        trips = await _context.Trip
                            .Where(t => t.KeerId != request.UserId)
                            .Where(t => t.Status == (int) TripStatus.Finding)
                            .Where(t => t.BookTime >= dateTime.AddMinutes(-15) && t.BookTime <= dateTime.AddMinutes(15))
                            .Where(t => t.Route.DepartureId == request.DepartureId)
                            .Where(t => t.Route.DestinationId == request.DestinationId)
                            .OrderBy(t => t.BookTime)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<TripDto>(_mapper.ConfigurationProvider,
                                new {isKeer = false})
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, trips.Count, lastPage, totalRecord
                    );

                    _logger.LogInformation("Successfully retrieved list of all upcoming trips for biker");
                    return Result<List<TripDto>>.Success(
                        trips, "Successfully retrieved list of all upcoming trips for biker.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<TripDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}