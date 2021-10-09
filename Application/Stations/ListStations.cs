using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
    public class ListStations
    {
        public class Query : IRequest<Result<List<StationDto>>>
        {
            public bool IsAdmin { get; set; }
            public int Page { get; set; }
            public int Limit { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<StationDto>>>
        {
            private readonly DataContext _context;
            private readonly ILogger<ListStations> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<ListStations> logger)
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

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must larger than 0");
                        return Result<List<StationDto>>.Failure("Page must larger than 0.");
                    }

                    int totalRecord = await _context.Station.CountAsync(cancellationToken);

                    #region Calculate last page

                    int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

                    #endregion

                    List<StationDto> stations = new();

                    if (request.Page <= lastPage)
                    {
                        if (request.IsAdmin)
                        {
                            stations = await _context.Station
                                .OrderBy(s => s.StationId)
                                .Skip((request.Page - 1) * request.Limit)
                                .Take(request.Limit)
                                .ProjectTo<StationDto>(_mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);
                        }
                        else
                        {
                            stations = await _context.Station
                                .Where(s => s.IsDeleted != true)
                                .OrderBy(s => s.StationId)
                                .Skip((request.Page - 1) * request.Limit)
                                .Take(request.Limit)
                                .ProjectTo<StationDto>(_mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);
                            // Set to null to make unnecessary fields excluded from response body.
                            stations.ForEach(s =>
                            {
                                s.CreatedDate = null;
                                s.IsDeleted = null;
                            });
                        }
                    }

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, stations.Count, lastPage, totalRecord
                    );

                    _logger.LogInformation("Successfully retrieved list of all stations");
                    return Result<List<StationDto>>.Success(
                        stations, "Successfully retrieved list of all stations.", paginationDto);
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