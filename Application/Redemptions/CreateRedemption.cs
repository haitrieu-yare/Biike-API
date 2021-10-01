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
			public RedemptionCreateDTO RedemptionCreateDTO { get; set; } = null!;
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
						.FindAsync(new object[] { request.RedemptionCreateDTO.VoucherId! }, cancellationToken);
					if (voucher == null)
					{
						_logger.LogInformation("Voucher doesn't exist");
						return Result<Unit>.Failure("Voucher doesn't exist");
					}

					// Check if voucher is expired
					if (CurrentTime.GetCurrentTime().CompareTo(voucher.EndDate) > 0)
					{
						_logger.LogInformation("Voucher is expired");
						return Result<Unit>.Failure("Voucher is expired");
					}

					// Check if voucher is open for redemption
					if (CurrentTime.GetCurrentTime().CompareTo(voucher.StartDate) < 0)
					{
						_logger.LogInformation("Voucher is not open for exchange yet");
						return Result<Unit>.Failure("Voucher is not open for exchange yet");
					}

					// Max number of active wallets is 2 for each user
					// Current Wallet
					var currentWallet = await _context.Wallet
						.Where(w => w.UserId == request.RedemptionCreateDTO.UserId)
						.Where(w => w.Status == (int)WalletStatus.Current)
						.SingleOrDefaultAsync(cancellationToken);
					// Old Wallet
					var oldWallet = await _context.Wallet
						.Where(w => w.UserId == request.RedemptionCreateDTO.UserId)
						.Where(w => w.Status == (int)WalletStatus.Old)
						.SingleOrDefaultAsync(cancellationToken);
					if (currentWallet == null)
					{
						_logger.LogInformation("User doesn't have wallet");
						return Result<Unit>.Failure("User doesn't have wallet");
					}

					var newRedemption = new Redemption();
					_mapper.Map(request.RedemptionCreateDTO, newRedemption);

					// Set voucherPoint
					newRedemption.VoucherPoint = voucher.AmountOfPoint;

					//TODO: Auto generate voucher code
					newRedemption.VoucherCode = "5XSG1205";

					// Change remaining of voucher
					voucher.Remaining--;

					if (oldWallet != null)
					{
						// Check if user have enough point
						int totalPoint = 0;
						totalPoint = oldWallet.Point + currentWallet.Point;

						if (voucher.AmountOfPoint - totalPoint < 0)
						{
							_logger.LogInformation("User doesn't have enough point");
							return Result<Unit>.Failure("User doesn't have enough point");
						}

						// Check if old wallet have enough point 
						// so we don't need to use current wallet point
						// Then minus the point 
						if (voucher.AmountOfPoint - oldWallet.Point >= 0)
						{
							// Set the old walletId
							newRedemption.WalletId = oldWallet.Id;
							oldWallet.Point -= voucher.AmountOfPoint;
						}
						else
						{
							// Set the current walletId
							newRedemption.WalletId = currentWallet.Id;
							int oldWalletPoint = oldWallet.Point;
							oldWalletPoint -= voucher.AmountOfPoint;

							if (oldWalletPoint < 0)
							{
								oldWallet.Point = 0;
								currentWallet.Point += oldWalletPoint;
							}
						}
					}

					if (oldWallet == null)
					{
						// Check if user have enough point
						if (voucher.AmountOfPoint - currentWallet.Point < 0)
						{
							_logger.LogInformation("User doesn't have enough point");
							return Result<Unit>.Failure("User doesn't have enough point");
						}

						// Set the current walletId
						newRedemption.WalletId = currentWallet.Id;

						// Minus point of user wallet
						currentWallet.Point -= voucher.AmountOfPoint;
					}

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