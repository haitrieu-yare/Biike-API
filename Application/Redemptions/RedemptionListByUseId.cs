using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RedemptionListByUseId
    {
        public class Query : IRequest<Result<List<RedemptionDto>>>
        {
            public int UserId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<RedemptionDto>>>
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

            public async Task<Result<List<RedemptionDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<RedemptionDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<RedemptionDto>>.Failure("Limit must be larger than 0.");
                    }

                    // Max number of active wallets is 2 for each user
                    List<Wallet> wallets = await _context.Wallet.Where(w => w.UserId == request.UserId)
                        .Where(w => w.Status != (int) WalletStatus.Expired)
                        .ToListAsync(cancellationToken);

                    if (wallets.Count == 0)
                    {
                        _logger.LogInformation("User doesn't have wallet");
                        return Result<List<RedemptionDto>>.Failure("User doesn't have wallet.");
                    }

                    var totalRecord = await _context.Redemption
                        .Where(r => r.WalletId == wallets[0].WalletId ||
                                    r.WalletId == (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId))
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<RedemptionDto> redemptions = new();

                    if (request.Page <= lastPage)
                        // Nếu user có 2 wallet thì query == wallets[0].Id || wallets[1].Id
                        // Nếu user có 1 wallet thì query == wallets[0].Id || wallets[0].Id
                        redemptions = await _context.Redemption
                            .Where(r => r.WalletId == wallets[0].WalletId || r.WalletId ==
                                (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId))
                            .OrderBy(r => r.RedemptionId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<RedemptionDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, redemptions.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved redemptions of userId {request.UserId}",
                        request.UserId);
                    return Result<List<RedemptionDto>>.Success(redemptions,
                        $"Successfully retrieved redemptions of userId {request.UserId}.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<RedemptionDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}