using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

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
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Wallet newWallet = new Wallet();

					_mapper.Map(request.WalletCreateDTO, newWallet);

					await _context.Wallet.AddAsync(newWallet, cancellationToken);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new wallet.");
						return Result<Unit>.Failure("Failed to create new wallet.");
					}
					else
					{
						_logger.LogInformation("Successfully created wallet.");
						return Result<Unit>.Success(
							Unit.Value, "Successfully created wallet.", newWallet.WalletId.ToString());
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