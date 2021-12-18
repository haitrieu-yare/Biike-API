using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WalletDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int WalletId { get; init; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Wallet? wallet =
                        await _context.Wallet.FindAsync(new object[] {request.WalletId}, cancellationToken);

                    if (wallet == null)
                    {
                        _logger.LogInformation("Wallet doesn't exist");
                        return Result<Unit>.NotFound("Wallet doesn't exist.");
                    }

                    _context.Wallet.Remove(wallet);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete wallet by walletId {request.WalletId}",
                            request.WalletId);
                        return Result<Unit>.Failure($"Failed to delete wallet by walletId {request.WalletId}.");
                    }

                    _logger.LogInformation("Successfully deleted wallet by walletId {request.WalletId}",
                        request.WalletId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted wallet by walletId {request.WalletId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) 
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}