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
            public string? Date { get; init; }
            public string? Time { get; init; }
        }

        // ReSharper disable once UnusedType.Global
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

                    var isDateProvided = !string.IsNullOrEmpty(request.Date);
                    var isTimeProvided = !string.IsNullOrEmpty(request.Time);
                    var isDepartureIdProvided = request.DepartureId > 0;
                    var isDestinationIdProvided = request.DestinationId > 0;

                    var date = CurrentTime.GetCurrentTime();
                    var time = date;
                    var currentTime = date;

                    if (isDateProvided && !DateTime.TryParse(request.Date, out date))
                    {
                        _logger.LogInformation("Date parameter format is invalid");
                        return Result<List<TripDto>>.Failure("Date parameter format is invalid.");
                    }

                    if (isTimeProvided && !DateTime.TryParse(request.Time, out time))
                    {
                        _logger.LogInformation("Time parameter format is invalid");
                        return Result<List<TripDto>>.Failure("Time parameter format is invalid.");
                    }

                    if (isDateProvided && date.CompareTo(currentTime.Date) < 0)
                    {
                        _logger.LogInformation("Date parameter value must be later than current time {CurrentTime}",
                            currentTime);
                        return Result<List<TripDto>>.Failure(
                            $"Date parameter value must be later than current time {currentTime}.");
                    }

                    if (isTimeProvided && (time.TimeOfDay.CompareTo(new TimeSpan(5, 0, 0)) < 0 ||
                                           time.TimeOfDay.CompareTo(new TimeSpan(21, 0, 0)) > 0))
                    {
                        _logger.LogInformation("Time parameter value must be later than 5AM and before 21PM");
                        return Result<List<TripDto>>.Failure(
                            "Time parameter value must be later than 5AM and before 21PM.");
                    }

                    var dateTime = date;
                    List<TripDto> trips = new();

                    if (isDateProvided && isTimeProvided)
                        dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

                    var totalRecord = await _context.Trip.Where(t => t.KeerId != request.UserId)
                        .Where(t => t.Status == (int) TripStatus.Finding)
                        .Where(t => (isDateProvided && isTimeProvided)
                            ?
                            t.BookTime >= dateTime.AddMinutes(-15) && t.BookTime <= dateTime.AddMinutes(15)
                            : (!isDateProvided && isTimeProvided)
                                ? (t.BookTime.TimeOfDay >= time.AddMinutes(-15).TimeOfDay &&
                                   t.BookTime.TimeOfDay <= time.AddMinutes(15).TimeOfDay)
                                : (isDateProvided && !isTimeProvided)
                                    ? (t.BookTime >= currentTime && t.BookTime <= date.Date.AddDays(1))
                                    : t.BookTime >= currentTime)
                        .Where(t => (isDepartureIdProvided && isDestinationIdProvided)
                            ?
                            (t.Route.DepartureId == request.DepartureId &&
                             t.Route.DestinationId == request.DestinationId)
                            : (isDepartureIdProvided && !isDestinationIdProvided)
                                ? t.Route.DepartureId == request.DepartureId
                                : (!isDepartureIdProvided && isDestinationIdProvided)
                                    ? t.Route.DestinationId == request.DestinationId
                                    : (t.Route.DestinationId > 0 && t.Route.DestinationId > 0))
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    if (request.Page <= lastPage)
                        trips = await _context.Trip.Where(t => t.KeerId != request.UserId)
                            .Where(t => t.Status == (int) TripStatus.Finding)
                            .Where(t => (isDateProvided && isTimeProvided)
                                ?
                                t.BookTime >= dateTime.AddMinutes(-15) && t.BookTime <= dateTime.AddMinutes(15)
                                : (!isDateProvided && isTimeProvided)
                                    ? (t.BookTime.TimeOfDay >= time.AddMinutes(-15).TimeOfDay &&
                                      t.BookTime.TimeOfDay <= time.AddMinutes(15).TimeOfDay)
                                    : (isDateProvided && !isTimeProvided)
                                        ? (t.BookTime >= currentTime && t.BookTime <= date.Date.AddDays(1))
                                        : t.BookTime >= currentTime)
                            .Where(t => (isDepartureIdProvided && isDestinationIdProvided)
                                ?
                                (t.Route.DepartureId == request.DepartureId &&
                                 t.Route.DestinationId == request.DestinationId)
                                : (isDepartureIdProvided && !isDestinationIdProvided)
                                    ? t.Route.DepartureId == request.DepartureId
                                    : (!isDepartureIdProvided && isDestinationIdProvided)
                                        ? t.Route.DestinationId == request.DestinationId
                                        : (t.Route.DestinationId > 0 && t.Route.DestinationId > 0))
                            .OrderBy(t => t.BookTime)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<TripDto>(_mapper.ConfigurationProvider, new {isKeer = false})
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, trips.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of newly created trips for biker");
                    return Result<List<TripDto>>.Success(trips,
                        "Successfully retrieved list of all newly created trips for biker.", paginationDto);
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