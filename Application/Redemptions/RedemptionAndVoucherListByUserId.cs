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
    public class RedemptionAndVoucherListByUserId
    {
        public class Query : IRequest<Result<List<RedemptionAndVoucherDto>>>
        {
            public int UserId { get; init; }
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<RedemptionAndVoucherDto>>>
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

            public async Task<Result<List<RedemptionAndVoucherDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<RedemptionAndVoucherDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<RedemptionAndVoucherDto>>.Failure("Limit must be larger than 0.");
                    }

                    // Max number of active wallets is 2 for each user
                    List<Wallet> wallets = await _context.Wallet.Where(w => w.UserId == request.UserId)
                        .Where(w => w.Status != (int) WalletStatus.Expired)
                        .ToListAsync(cancellationToken);

                    if (wallets.Count == 0)
                    {
                        _logger.LogInformation("User doesn't have wallet");
                        return Result<List<RedemptionAndVoucherDto>>.Failure("User doesn't have wallet.");
                    }

                    var totalRecord = await _context.Redemption
                        .Where(r => r.WalletId == wallets[0].WalletId ||
                                    r.WalletId == (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId))
                        .CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<RedemptionAndVoucherDto> redemptions = new();

                    if (request.Page <= lastPage)
                        // Nếu user có 2 wallet thì query == wallets[0].Id || wallets[1].Id
                        // Nếu user có 1 wallet thì query == wallets[0].Id || wallets[0].Id
                        redemptions = await _context.Redemption
                            .Where(r => r.WalletId == wallets[0].WalletId || r.WalletId ==
                                (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId))
                            .OrderBy(r => r.RedemptionId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<RedemptionAndVoucherDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, redemptions.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved redemptions and vouchers of userId {request.UserId}",
                        request.UserId);
                    return Result<List<RedemptionAndVoucherDto>>.Success(redemptions,
                        "Successfully retrieved redemptions and " + $"vouchers of userId {request.UserId}.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<RedemptionAndVoucherDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}