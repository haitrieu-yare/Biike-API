using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RedemptionDetailsFull
    {
        public class Query : IRequest<Result<RedemptionAndVoucherDto>>
        {
            public int RedemptionId { get; init; }
            public int UserRequestId { get; init; }
            public bool IsAdmin { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<RedemptionAndVoucherDto>>
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

            public async Task<Result<RedemptionAndVoucherDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Domain.Entities.Redemption redemptionDb = await _context.Redemption.Include(r => r.Voucher)
                        .Include(r => r.Wallet.User)
                        .Where(r => r.RedemptionId == request.RedemptionId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (redemptionDb == null)
                    {
                        _logger.LogInformation("Redemption doesn't exist");
                        return Result<RedemptionAndVoucherDto>.NotFound("Redemption doesn't exist.");
                    }

                    if (redemptionDb.Wallet.UserId != request.UserRequestId && !request.IsAdmin)
                    {
                        _logger.LogInformation(
                            "Redemption with RedemptionId {request.RedemptionId} doesn't belong to user with {request.UserRequestId}",
                            request.RedemptionId, request.UserRequestId);
                        return Result<RedemptionAndVoucherDto>.NotFound(
                            $"Redemption with RedemptionId {request.RedemptionId} doesn't belong to user with {request.UserRequestId}.");
                    }

                    RedemptionAndVoucherDto redemption = new();

                    _mapper.Map(redemptionDb, redemption);

                    _logger.LogInformation("Successfully retrieved redemption by {request.RedemptionId}",
                        request.RedemptionId);
                    return Result<RedemptionAndVoucherDto>.Success(redemption,
                        $"Successfully retrieved redemption by {request.RedemptionId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<RedemptionAndVoucherDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}