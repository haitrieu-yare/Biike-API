using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Wallets.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Wallets
{
	public class CreateWallet
	{
		public class Command : IRequest<Result<Unit>>
		{
			public WalletCreateDto WalletCreateDto { get; init; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
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

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Wallet newWallet = new();

					_mapper.Map(request.WalletCreateDto, newWallet);

					await _context.Wallet.AddAsync(newWallet, cancellationToken);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new wallet");
						return Result<Unit>.Failure("Failed to create new wallet.");
					}

					_logger.LogInformation("Successfully created wallet");
					return Result<Unit>.Success(
						Unit.Value, "Successfully created wallet.", newWallet.WalletId.ToString());
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}