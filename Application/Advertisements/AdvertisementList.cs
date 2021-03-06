using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Advertisements.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementList
    {
        public class Query : IRequest<Result<List<AdvertisementDto>>>
        {
            public Query(int page, int limit, bool isAdmin)
            {
                Page = page;
                Limit = limit;
                IsAdmin = isAdmin;
            }

            public int Page { get; }
            public int Limit { get; }
            public bool IsAdmin { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<AdvertisementDto>>>
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

            public async Task<Result<List<AdvertisementDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<AdvertisementDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<AdvertisementDto>>.Failure("Limit must be larger than 0.");
                    }

                    var query = _context.Advertisement.AsQueryable();

                    if (!request.IsAdmin)
                    {
                        query = query.Where(a => a.IsActive == true);
                    }

                    var totalRecord = await query.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<AdvertisementDto> advertisements = new();

                    if (request.Page <= lastPage)
                        advertisements = await query.AsSingleQuery()
                            .OrderBy(v => v.AdvertisementId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<AdvertisementDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, advertisements.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all advertisements");
                    return Result<List<AdvertisementDto>>.Success(advertisements,
                        "Successfully retrieved list of all advertisements.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<AdvertisementDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}