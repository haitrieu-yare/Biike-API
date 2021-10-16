using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Intimacies.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IntimacyListByUserId
    {
        public class Query : IRequest<Result<List<IntimacyDto>>>
        {
            public int UserOneId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<IntimacyDto>>>
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

            public async Task<Result<List<IntimacyDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<IntimacyDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<IntimacyDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Intimacy.Where(i => i.UserOneId == request.UserOneId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<IntimacyDto> intimacies = new();

                    if (request.Page <= lastPage)
                        intimacies = await _context.Intimacy.Where(i => i.UserOneId == request.UserOneId)
                            .OrderBy(i => i.UserOneId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<IntimacyDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, intimacies.Count, lastPage, totalRecord);

                    _logger.LogInformation(
                        "Successfully retrieved list of user intimacies by UserId {request.UserOneId}",
                        request.UserOneId);
                    return Result<List<IntimacyDto>>.Success(intimacies,
                        "Successfully retrieved list of " + $"user intimacies by UserId {request.UserOneId}.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<IntimacyDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}