using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using Persistence;
using Application.Core;
using Application.Wallets.DTOs;
using Domain.Entities;

namespace Application.Wallets
{
	public class CreateWallet
	{
		public class Command : IRequest<Result<Unit>>
		{
			public WalletCreateDTO WalletCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CreateWallet> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateWallet> logger)
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

					var newWallet = new Wallet();
					_mapper.Map(request.WalletCreateDTO, newWallet);

					await _context.Wallet.AddAsync(newWallet, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new wallet");
						return Result<Unit>.Failure("Failed to create new wallet");
					}
					else
					{
						_logger.LogInformation("Successfully created wallet");
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