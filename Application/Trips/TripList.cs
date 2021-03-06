using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripList
    {
        public class Query : IRequest<Result<List<TripDetailsDto>>>
        {
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<TripDetailsDto>>>
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

            public async Task<Result<List<TripDetailsDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<TripDetailsDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<TripDetailsDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Trip.CountAsync(cancellationToken);
                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<TripDetailsDto> trips = new();

                    if (request.Page <= lastPage)
                        trips = await _context.Trip
                            .OrderBy(t => t.TripId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<TripDetailsDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, trips.Count, lastPage, totalRecord
                    );

                    _logger.LogInformation("Successfully retrieved list of all trips");
                    return Result<List<TripDetailsDto>>.Success(
                        trips, "Successfully retrieved list of all trips.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<TripDetailsDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}