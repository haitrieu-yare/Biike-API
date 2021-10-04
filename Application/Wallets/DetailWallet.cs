using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Wallets
{
	public class DetailWallet
	{
		public class Query : IRequest<Result<WalletDTO>>
		{
			public int WalletId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<WalletDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailWallet> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailWallet> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<WalletDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var walletDB = await _context.Wallet
						.FindAsync(new object[] { request.WalletId }, cancellationToken);

					WalletDTO wallet = new WalletDTO();

					_mapper.Map(walletDB, wallet);

					_logger.LogInformation($"Successfully retrieved wallet by walletId {request.WalletId}.");
					return Result<WalletDTO>.Success(
						wallet, $"Successfully retrieved wallet by walletId {request.WalletId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<WalletDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}