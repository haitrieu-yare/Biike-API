using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.BikeAvailabilities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.BikeAvailabilities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeAvailabilityListByUserId
    {
        public class Query : IRequest<Result<List<BikeAvailabilityDto>>>
        {
            public Query(int page, int limit, int userId)
            {
                Page = page;
                Limit = limit;
                UserId = userId;
            }

            public int Page { get; }
            public int Limit { get; }
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<BikeAvailabilityDto>>>
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

            public async Task<Result<List<BikeAvailabilityDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<BikeAvailabilityDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<BikeAvailabilityDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.BikeAvailability
                        .Where(b => b.UserId == request.UserId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<BikeAvailabilityDto> bikeAvailabilities = new();

                    if (request.Page <= lastPage)
                        bikeAvailabilities = await _context.BikeAvailability
                            .Where(b => b.UserId == request.UserId)
                            .OrderBy(i => i.BikeAvailabilityId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<BikeAvailabilityDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, bikeAvailabilities.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all bike availabilities by UserId");
                    return Result<List<BikeAvailabilityDto>>.Success(bikeAvailabilities,
                        "Successfully retrieved list of all bike availabilities by UserId.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<BikeAvailabilityDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}