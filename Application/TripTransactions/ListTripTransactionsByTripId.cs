using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
    public class ListTripTransactionsByTripId
    {
        public class Query : IRequest<Result<List<TripTransactionDto>>>
        {
            public int TripId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TripTransactionDto>>>
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

            public async Task<Result<List<TripTransactionDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<TripTransactionDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<TripTransactionDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.TripTransaction
                        .Where(t => t.TripId == request.TripId)
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<TripTransactionDto> tripTransaction = new();

                    if (request.Page <= lastPage)
                        tripTransaction = await _context.TripTransaction
                            .Where(t => t.TripId == request.TripId)
                            .ProjectTo<TripTransactionDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    _logger.LogInformation("Successfully retrieved trip transaction " +
                                           "based on tripId {request.TripId}", request.TripId);
                    return Result<List<TripTransactionDto>>.Success(
                        tripTransaction, "Successfully retrieved trip transaction " +
                                         $"based on tripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<TripTransactionDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}