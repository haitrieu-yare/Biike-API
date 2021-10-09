using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
    public class EditWallet
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int WalletId { get; set; }
            public WalletDto NewWalletDto { get; set; } = null!;
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<EditWallet> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<EditWallet> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var oldWallet = await _context.Wallet
                        .FindAsync(new object[] {request.WalletId}, cancellationToken);

                    if (oldWallet == null) return null!;

                    _mapper.Map(request.NewWalletDto, oldWallet);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation($"Failed to update wallet by walletId {request.WalletId}.");
                        return Result<Unit>.Failure($"Failed to update wallet by walletId {request.WalletId}.");
                    }

                    _logger.LogInformation($"Successfully updated wallet by walletId {request.WalletId}.");
                    return Result<Unit>.Success(
                        Unit.Value, $"Successfully updated wallet by walletId {request.WalletId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled.");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}