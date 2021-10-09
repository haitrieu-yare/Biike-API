using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
	public class AutoCreateTripTransaction
	{
		private readonly DataContext _context;
		private readonly ILogger<AutoCreateTripTransaction> _logger;
		private readonly IMapper _mapper;

		public AutoCreateTripTransaction(DataContext context, IMapper mapper, ILogger<AutoCreateTripTransaction> logger)
		{
			_context = context;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Run(Trip trip, int newPoint, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var tripTransaction = new TripTransaction
			{
				TripId = trip.TripId,
				TransactionDate = CurrentTime.GetCurrentTime()
			};

			var wallet = await _context.Wallet
				.Where(w => w.UserId == trip.BikerId)
				.Where(w => w.Status == (int) WalletStatus.Current)
				.SingleOrDefaultAsync(cancellationToken);

			tripTransaction.WalletId = wallet.WalletId;
			tripTransaction.AmountOfPoint = newPoint;
			wallet.Point += newPoint;

			var user = await _context.User
				.Where(u => u.UserId == wallet.UserId)
				.SingleOrDefaultAsync(cancellationToken);

			user.TotalPoint += newPoint;

			await _context.TripTransaction.AddAsync(tripTransaction, cancellationToken);

			// Save change to feedback, tripTransaction, wallet, user table
			var result = await _context.SaveChangesAsync(cancellationToken) > 0;

			if (!result)
			{
				_logger.LogInformation("Failed to create new trip transaction.");
				throw new Exception("Failed to create new trip transaction.");
			}

			_logger.LogInformation("Successfully created trip transaction.");
		}
	}
}