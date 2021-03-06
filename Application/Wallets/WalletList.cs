using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WalletList
    {
        public class Query : IRequest<Result<List<WalletDto>>>
        {
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<WalletDto>>>
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

            public async Task<Result<List<WalletDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<WalletDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<WalletDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Wallet.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<WalletDto> wallets = new();

                    if (request.Page <= lastPage)
                        wallets = await _context.Wallet
                            .OrderBy(w => w.WalletId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<WalletDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, wallets.Count, lastPage, totalRecord
                    );

                    _logger.LogInformation("Successfully retrieved list of all wallets");
                    return Result<List<WalletDto>>.Success(
                        wallets, "Successfully retrieved list of all wallets.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<WalletDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}