using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpcomingExpiredWallet
    {
        public class Query : IRequest<Result<WalletDto>>
        {
            public Query(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<WalletDto>>
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

            public async Task<Result<WalletDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var wallet = await _context.Wallet.Where(w => w.UserId == request.UserId)
                        .Where(w => w.Status == (int) WalletStatus.Old)
                        .ProjectTo<WalletDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (wallet == null)
                    {
                        _logger.LogInformation("Wallet doesn't exist");
                        return Result<WalletDto>.NotFound("Wallet doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved upcoming expired wallet by userId {UserId}",
                        request.UserId);
                    return Result<WalletDto>.Success(wallet,
                        $"Successfully retrieved upcoming expired wallet by userId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<WalletDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}