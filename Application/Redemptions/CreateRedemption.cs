using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Enums;
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
			public RedemptionCreateDto RedemptionCreateDto { get; set; } = null!;
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

					User user = await _context.User.FindAsync(new object[] { request.RedemptionCreateDto.UserId! },
						cancellationToken);

					if (user == null)
					{
						_logger.LogInformation("User with UserId {UserId} doesn't exist",
							request.RedemptionCreateDto.UserId);
						return Result<Unit>.NotFound(
							$"User with UserId {request.RedemptionCreateDto.UserId} doesn't exist.");
					}

					Voucher voucher =
						await _context.Voucher.FindAsync(new object[] { request.RedemptionCreateDto.VoucherId! },
							cancellationToken);

					if (voucher == null)
					{
						_logger.LogInformation("Voucher doesn't exist");
						return Result<Unit>.NotFound("Voucher doesn't exist.");
					}

					// Check if voucher is expired
					if (CurrentTime.GetCurrentTime().CompareTo(voucher.EndDate) > 0)
					{
						_logger.LogInformation("Voucher has expired");
						return Result<Unit>.Failure("Voucher has expired.");
					}

					// Check if voucher is open for redemption
					if (CurrentTime.GetCurrentTime().CompareTo(voucher.StartDate) < 0)
					{
						_logger.LogInformation("Voucher is not open for exchange yet");
						return Result<Unit>.Failure("Voucher is not open for exchange yet.");
					}

					// Max number of active wallets is 2 for each user
					// Current Wallet
					Wallet currentWallet = await _context.Wallet
						.Where(w => w.UserId == request.RedemptionCreateDto.UserId)
						.Where(w => w.Status == (int) WalletStatus.Current)
						.SingleOrDefaultAsync(cancellationToken);
					// Old Wallet
					Wallet oldWallet = await _context.Wallet.Where(w => w.UserId == request.RedemptionCreateDto.UserId)
						.Where(w => w.Status == (int) WalletStatus.Old)
						.SingleOrDefaultAsync(cancellationToken);

					if (currentWallet == null)
					{
						_logger.LogInformation("User doesn't have wallet");
						return Result<Unit>.Failure("User doesn't have wallet.");
					}

					Redemption newRedemption = new();

					_mapper.Map(request.RedemptionCreateDto, newRedemption);

					// Set voucherPoint
					newRedemption.VoucherPoint = voucher.AmountOfPoint;

					//TODO: Auto generate voucher code
					newRedemption.VoucherCode = "5XSG1205";

					// Change remaining of voucher
					voucher.Remaining--;

					if (oldWallet != null)
					{
						// Check if user have enough point
						int totalPoint = oldWallet.Point + currentWallet.Point;

						if (totalPoint - voucher.AmountOfPoint < 0)
						{
							_logger.LogInformation("User doesn't have enough point");
							return Result<Unit>.Failure("User doesn't have enough point.");
						}

						// Check if old wallet have enough point 
						// so we don't need to use current wallet point
						if (oldWallet.Point - voucher.AmountOfPoint >= 0)
						{
							// Set the old walletId
							newRedemption.WalletId = oldWallet.WalletId;
							oldWallet.Point -= voucher.AmountOfPoint;
						}
						else
						{
							// Set the current walletId
							newRedemption.WalletId = currentWallet.WalletId;

							// Use point of old wallet
							oldWallet.Point -= voucher.AmountOfPoint;

							// Use point of current wallet
							currentWallet.Point += oldWallet.Point;

							// Set point of old wallet back to 0
							oldWallet.Point = 0;
						}
					}

					if (oldWallet == null)
					{
						// Check if user have enough point
						if (currentWallet.Point - voucher.AmountOfPoint < 0)
						{
							_logger.LogInformation("User doesn't have enough point");
							return Result<Unit>.Failure("User doesn't have enough point.");
						}

						// Set the current walletId
						newRedemption.WalletId = currentWallet.WalletId;

						// Minus point of user wallet
						currentWallet.Point -= voucher.AmountOfPoint;
					}

					// Update totalPoint of user
					user.TotalPoint = (oldWallet?.Point ?? 0) + currentWallet.Point;

					await _context.Redemption.AddAsync(newRedemption, cancellationToken);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new redemption");
						return Result<Unit>.Failure("Failed to create new redemption.");
					}

					_logger.LogInformation("Successfully created redemption");
					return Result<Unit>.Success(Unit.Value, "Successfully created redemption.",
						newRedemption.RedemptionId.ToString());
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