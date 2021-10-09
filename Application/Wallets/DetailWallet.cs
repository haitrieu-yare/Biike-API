using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
    public class DetailWallet
    {
        public class Query : IRequest<Result<WalletDto>>
        {
            public int WalletId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<WalletDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<DetailWallet> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<DetailWallet> logger)
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

                    var walletDb = await _context.Wallet
                        .FindAsync(new object[] {request.WalletId}, cancellationToken);

                    WalletDto wallet = new();

                    _mapper.Map(walletDb, wallet);

                    _logger.LogInformation($"Successfully retrieved wallet by walletId {request.WalletId}.");
                    return Result<WalletDto>.Success(
                        wallet, $"Successfully retrieved wallet by walletId {request.WalletId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled.");
                    return Result<WalletDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}