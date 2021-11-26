using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Configurations.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Configurations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConfigurationList
    {
        public class Query : IRequest<Result<List<ConfigurationDto>>>
        {
            public Query(int page, int limit)
            {
                Page = page;
                Limit = limit;
            }

            public int Page { get; }
            public int Limit { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<ConfigurationDto>>>
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

            public async Task<Result<List<ConfigurationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<ConfigurationDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<ConfigurationDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Configuration.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<ConfigurationDto> configurations = new();

                    if (request.Page <= lastPage)
                        configurations = await _context.Configuration
                            .OrderBy(i => i.ConfigurationId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<ConfigurationDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, configurations.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all configurations");
                    return Result<List<ConfigurationDto>>.Success(configurations,
                        "Successfully retrieved list of all configurations.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<ConfigurationDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}