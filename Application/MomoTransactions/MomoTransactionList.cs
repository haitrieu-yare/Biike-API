﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.MomoTransactions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.MomoTransactions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MomoTransactionList
    {
        public class Query : IRequest<Result<List<MomoTransactionDto>>>
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
        public class Handler : IRequestHandler<Query, Result<List<MomoTransactionDto>>>
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

            public async Task<Result<List<MomoTransactionDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<MomoTransactionDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<MomoTransactionDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.MomoTransaction.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<MomoTransactionDto> momoTransactions = new();

                    if (request.Page <= lastPage)
                        momoTransactions = await _context.MomoTransaction.OrderBy(i => i.MomoTransactionId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<MomoTransactionDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, momoTransactions.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all Momo Transactions");
                    return Result<List<MomoTransactionDto>>.Success(momoTransactions,
                        "Successfully retrieved list of all Momo Transactions.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<MomoTransactionDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}