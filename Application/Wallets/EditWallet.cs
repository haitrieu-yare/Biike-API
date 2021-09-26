using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using Persistence;
using Application.Core;
using Application.Wallets.DTOs;

namespace Application.Wallets
{
	public class EditWallet
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int WalletId { get; set; }
			public WalletDTO NewWalletDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditWallet> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditWallet> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldWallet = await _context.Wallet
						.FindAsync(new object[] { request.WalletId }, cancellationToken);
					if (oldWallet == null) return null!;

					_mapper.Map(request.NewWalletDTO, oldWallet);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update wallet by walletId: " + request.WalletId);
						return Result<Unit>.Failure("Failed to update wallet by walletId: " + request.WalletId);
					}
					else
					{
						_logger.LogInformation("Successfully updated wallet by walletId: " + request.WalletId);
						return Result<Unit>.Success(Unit.Value);
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