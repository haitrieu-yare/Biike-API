using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using Persistence;
using Application.Core;

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
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var wallet = await _context.Wallet
						.Where(w => w.WalletId == request.WalletId)
						.SingleOrDefaultAsync(cancellationToken);
					if (wallet == null) return null!;

					_context.Wallet.Remove(wallet);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to delete wallet by walletId: " + request.WalletId);
						return Result<Unit>.Failure("Failed to delete wallet by walletId: " + request.WalletId);
					}
					else
					{
						_logger.LogInformation("Successfully deleted wallet by walletId: " + request.WalletId);
						return Result<Unit>
							.Success(Unit.Value, "Successfully deleted wallet by walletId: " + request.WalletId);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}