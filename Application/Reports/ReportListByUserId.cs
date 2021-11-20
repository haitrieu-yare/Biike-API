using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Reports.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Reports
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReportListByUserId
    {
        public class Query : IRequest<Result<List<ReportDto>>>
        {
            public Query(int userOneId, int page, int limit)
            {
                UserOneId = userOneId;
                Page = page;
                Limit = limit;
            }

            public int UserOneId { get; }
            public int Page { get; }
            public int Limit { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<ReportDto>>>
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

            public async Task<Result<List<ReportDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<ReportDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<ReportDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Report
                        .Where(i => i.UserOneId == request.UserOneId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<ReportDto> reports = new();

                    if (request.Page <= lastPage)
                        reports = await _context.Report
                            .Where(i => i.UserOneId == request.UserOneId)
                            .OrderBy(i => i.UserTwoId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<ReportDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, reports.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of user reports by UserId {request.UserOneId}",
                        request.UserOneId);
                    return Result<List<ReportDto>>.Success(reports,
                        "Successfully retrieved list of " + $"user reports by UserId {request.UserOneId}.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<ReportDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}