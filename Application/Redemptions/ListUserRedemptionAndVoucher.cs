using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Application.Core;
using Application.Redemptions.DTOs;
using Persistence;
using Domain.Enums;

namespace Application.Redemptions
{
	public class ListUserRedemptionAndVoucher
	{
		public class Query : IRequest<Result<List<RedemptionAndVoucherDTO>>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RedemptionAndVoucherDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListUserRedemption> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListUserRedemption> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<List<RedemptionAndVoucherDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					// Max number of active wallets is 2 for each user
					var wallets = await _context.Wallet
						.Where(w => w.UserId == request.UserId)
						.Where(w => w.Status != (int)WalletStatus.Expired)
						.ToListAsync(cancellationToken);
					if (wallets == null)
					{
						_logger.LogInformation("User doesn't have wallet");
						return Result<List<RedemptionAndVoucherDTO>>.Failure("User doesn't have wallet");
					}

					// Nếu user có 2 wallet thì query == wallets[0].Id | wallets[1].Id
					// Nếu user có 1 wallet thì query == wallets[0].Id | wallets[0].Id
					var redemptions = await _context.Redemption
						.Where(r => r.WalletId == wallets[0].Id ||
							r.WalletId == (wallets.Count == 2 ? wallets[1].Id : wallets[0].Id)
						)
						.ProjectTo<RedemptionAndVoucherDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved redemptions and vouchers of userId: " + request.UserId);
					return Result<List<RedemptionAndVoucherDTO>>.Success(redemptions);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RedemptionAndVoucherDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}