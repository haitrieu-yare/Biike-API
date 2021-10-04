using System.Collections.Generic;
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
	public class ListWallets
	{
		public class Query : IRequest<Result<List<WalletDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<WalletDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListWallets> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListWallets> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<WalletDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var wallets = await _context.Wallet
						.ProjectTo<WalletDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all wallets");
					return Result<List<WalletDTO>>
						.Success(wallets, "Successfully retrieved list of all wallets");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<WalletDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}