using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Wallets
{
	public class DeleteWallet
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int WalletId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteWallet> _logger;
			public Handler(DataContext context, ILogger<DeleteWallet> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var wallet = await _context.Wallet
						.FindAsync(new object[] { request.WalletId }, cancellationToken);

					if (wallet == null) return null!;

					_context.Wallet.Remove(wallet);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to delete wallet by walletId {request.WalletId}.");
						return Result<Unit>.Failure($"Failed to delete wallet by walletId {request.WalletId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully deleted wallet by walletId {request.WalletId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully deleted wallet by walletId {request.WalletId}.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}