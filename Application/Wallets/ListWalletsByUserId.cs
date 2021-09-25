using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Persistence;
using Application.Core;
using Application.Wallets.DTOs;

namespace Application.Wallets
{
	public class ListWalletsByUserId
	{
		public class Query : IRequest<Result<List<WalletDTO>>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<WalletDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListWalletsByUserId> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListWalletsByUserId> logger)
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
						.Where(w => w.AppUserId == request.UserId)
						.ProjectTo<WalletDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all user's wallets");
					return Result<List<WalletDTO>>.Success(wallets);
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