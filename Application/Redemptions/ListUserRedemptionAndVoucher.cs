using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Redemptions
{
	public class ListUserRedemptionAndVoucher
	{
		public class Query : IRequest<Result<List<RedemptionAndVoucherDTO>>>
		{
			public int UserId { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RedemptionAndVoucherDTO>>>
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

			public async Task<Result<List<RedemptionAndVoucherDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<RedemptionAndVoucherDTO>>.Failure("Page must larger than 0.");
					}

					// Max number of active wallets is 2 for each user
					var wallets = await _context.Wallet
						.Where(w => w.UserId == request.UserId)
						.Where(w => w.Status != (int)WalletStatus.Expired)
						.ToListAsync(cancellationToken);

					if (wallets == null)
					{
						_logger.LogInformation("User doesn't have wallet");
						return Result<List<RedemptionAndVoucherDTO>>.Failure("User doesn't have wallet.");
					}

					int totalRecord = await _context.Redemption
						.Where(r => r.WalletId == wallets[0].WalletId ||
							r.WalletId == (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId)
						).CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<RedemptionAndVoucherDTO> redemptions = new List<RedemptionAndVoucherDTO>();

					if (request.Page <= lastPage)
					{
						// Nếu user có 2 wallet thì query == wallets[0].Id || wallets[1].Id
						// Nếu user có 1 wallet thì query == wallets[0].Id || wallets[0].Id
						redemptions = await _context.Redemption
							.Where(r => r.WalletId == wallets[0].WalletId ||
								r.WalletId == (wallets.Count == 2 ? wallets[1].WalletId : wallets[0].WalletId)
							)
							.OrderBy(r => r.RedemptionId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<RedemptionAndVoucherDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, redemptions.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved redemptions and " +
						$"vouchers of userId {request.UserId}");
					return Result<List<RedemptionAndVoucherDTO>>.Success
						(redemptions, "Successfully retrieved redemptions and " +
						$"vouchers of userId {request.UserId}.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RedemptionAndVoucherDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}