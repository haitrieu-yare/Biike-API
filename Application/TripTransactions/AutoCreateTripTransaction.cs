using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public AutoCreateTripTransaction(DataContext context, ILogger<AutoCreateTripTransaction> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Run(Trip trip, int newPoint, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (trip.BikerId == null)
            {
                _logger.LogInformation("Trip with TripId {TripId} doesn't have Biker", trip.TripId);
                throw new Exception($"Trip with TripId {trip.TripId} doesn't have Biker.");
            }

            User user = await _context.User.FindAsync(new object[] {trip.BikerId}, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("User with UserId {UserId} doesn't exist", trip.BikerId);
                throw new Exception($"User with UserId {trip.BikerId} doesn't exist.");
            }

            Wallet currentWallet = await _context.Wallet.Where(w => w.UserId == trip.BikerId)
                .Where(w => w.Status == (int) WalletStatus.Current)
                .SingleOrDefaultAsync(cancellationToken);

            if (currentWallet == null)
            {
                _logger.LogInformation("Biker with UserId {UserId} doesn't have wallet", trip.BikerId);
                throw new Exception($"Biker with UserId {trip.BikerId} doesn't have wallet.");
            }

            var tripTransaction = new TripTransaction
            {
                TripId = trip.TripId,
                TransactionDate = CurrentTime.GetCurrentTime(),
                WalletId = currentWallet.WalletId,
                AmountOfPoint = newPoint
            };

            await _context.TripTransaction.AddAsync(tripTransaction, cancellationToken);

            // Add point to current wallet
            currentWallet.Point += newPoint;

            // Update user total point
            user.TotalPoint += newPoint;

            // Save change to 4 tables: feedback, tripTransaction, wallet, user
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogInformation("Failed to create new trip transaction");
                throw new Exception("Failed to create new trip transaction.");
            }

            _logger.LogInformation("Successfully created trip transaction");
        }
    }
}