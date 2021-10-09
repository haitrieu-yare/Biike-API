using System.Collections.Generic;
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
	public class ListWallets
	{
		public class Query : IRequest<Result<List<WalletDto>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<WalletDto>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<WalletDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<WalletDto>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<WalletDto>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.Wallet.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<WalletDto> wallets = new List<WalletDto>();

					if (request.Page <= lastPage)
					{
						wallets = await _context.Wallet
							.OrderBy(w => w.WalletId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<WalletDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDto paginationDto = new PaginationDto(
						request.Page, request.Limit, wallets.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all wallets");
					return Result<List<WalletDto>>.Success(
						wallets, "Successfully retrieved list of all wallets.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<WalletDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}