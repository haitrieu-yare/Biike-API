using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
	public class CreateRedemption
	{
		public class Command : IRequest<Result<Unit>>
		{
			public RedemptionCreateDTO RedemptionCreateDTO { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CreateRedemption> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateRedemption> logger)
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

					var voucher = await _context.Voucher
						.FindAsync(new object[] { request.RedemptionCreateDTO.VoucherId }, cancellationToken);
					if (voucher == null)
					{
						_logger.LogInformation("Voucher doesn't exist");
						return Result<Unit>.Failure("Voucher doesn't exist");
					}

					var wallet = await _context.Wallet
						.FindAsync(new object[] { request.RedemptionCreateDTO.WalletId }, cancellationToken);
					if (wallet == null)
					{
						_logger.LogInformation("User's wallet doesn't exist");
						return Result<Unit>.Failure("User's wallet doesn't exist");
					}

					var newRedemption = new Redemption();
					_mapper.Map(request.RedemptionCreateDTO, newRedemption);
					newRedemption.IsUsed = false;
					newRedemption.VoucherPoint = voucher.AmountOfPoint;
					//TODO: Auto generate voucher code
					newRedemption.VoucherCode = "5XSG1205";

					// Change remaining of voucher
					voucher.Remaining--;
					// Minus point of user wallet
					wallet.Point -= voucher.AmountOfPoint;

					await _context.Redemption.AddAsync(newRedemption, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new redemption");
						return Result<Unit>.Failure("Failed to create new redemption");
					}
					else
					{
						_logger.LogInformation("Successfully created redemption");
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