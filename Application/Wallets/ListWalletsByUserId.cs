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
	public class ListWalletsByUserId
	{
		public class Query : IRequest<Result<List<WalletDTO>>>
		{
			public int UserId { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<WalletDTO>>>
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

			public async Task<Result<List<WalletDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<WalletDTO>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<WalletDTO>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.Wallet
						.Where(w => w.UserId == request.UserId)
						.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<WalletDTO> wallets = new List<WalletDTO>();

					if (request.Page <= lastPage)
					{
						wallets = await _context.Wallet
							.Where(w => w.UserId == request.UserId)
							.OrderBy(w => w.WalletId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<WalletDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, wallets.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all user's wallets");
					return Result<List<WalletDTO>>.Success(
						wallets, "Successfully retrieved list of all user's wallets.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<WalletDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}