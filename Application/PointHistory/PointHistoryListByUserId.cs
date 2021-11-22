using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.PointHistory.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.PointHistory
{
    public class PointHistoryListByUserId
    {
        public class Query : IRequest<Result<List<PointHistoryDto>>>
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
        public class Handler : IRequestHandler<Query, Result<List<PointHistoryDto>>>
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

            public async Task<Result<List<PointHistoryDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<PointHistoryDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<PointHistoryDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.PointHistory.Where(p => p.UserId == request.UserId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<PointHistoryDto> pointHistoryList = new();

                    if (request.Page <= lastPage)
                        pointHistoryList = await _context.PointHistory
                            .Where(p => p.UserId == request.UserId)
                            .OrderByDescending(r => r.TimeStamp)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<PointHistoryDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, pointHistoryList.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all point history by UserId {UserId}",
                        request.UserId);
                    return Result<List<PointHistoryDto>>.Success(pointHistoryList,
                        $"Successfully retrieved list of all point history by UserId {request.UserId}.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<PointHistoryDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}