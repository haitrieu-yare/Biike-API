using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Sos.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Sos
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SosList
    {
        public class Query : IRequest<Result<List<SosDto>>>
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
        public class Handler : IRequestHandler<Query, Result<List<SosDto>>>
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

            public async Task<Result<List<SosDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<SosDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<SosDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Sos.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);
                    List<SosDto> sosList = new();

                    if (request.Page <= lastPage)
                    {
                        sosList = await _context.Sos.OrderBy(s => s.SosId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<SosDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                        if (!request.IsAdmin)
                        {
                            // Set to null to make unnecessary fields excluded from the response body.
                            sosList.ForEach(s => { s.CreatedDate = null; });
                        }
                    }

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, sosList.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all sos");
                    return Result<List<SosDto>>.Success(sosList, "Successfully retrieved list of all sos.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<SosDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}