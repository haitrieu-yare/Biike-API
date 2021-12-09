using System;
using System.Linq;
using System.Threading.Tasks;
using Application.PointHistory;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
    public class AutoTripTransactionCreation
    {
        private readonly DataContext _context;
        private readonly AutoPointHistoryCreation _pointHistoryCreation;
        private readonly ILogger<AutoTripTransactionCreation> _logger;

        public AutoTripTransactionCreation(DataContext context, AutoPointHistoryCreation pointHistoryCreation,
            ILogger<AutoTripTransactionCreation> logger)
        {
            _context = context;
            _pointHistoryCreation = pointHistoryCreation;
            _logger = logger;
        }

        public async Task Run(Trip trip, int newPoint, string description)
        {
            if (trip.BikerId == null)
            {
                _logger.LogInformation("Trip with TripId {TripId} doesn't have Biker", trip.TripId);
                throw new Exception($"Trip with TripId {trip.TripId} doesn't have Biker.");
            }

            User biker = await _context.User.FindAsync(trip.BikerId);

            if (biker == null || biker.IsDeleted)
            {
                _logger.LogInformation("Biker with UserId {UserId} doesn't exist", trip.BikerId);
                throw new Exception($"Biker with UserId {trip.BikerId} doesn't exist.");
            }

            Wallet currentWallet = await _context.Wallet
                .Where(w => w.UserId == trip.BikerId)
                .Where(w => w.Status == (int) WalletStatus.Current)
                .SingleOrDefaultAsync();

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
                AmountOfPoint = newPoint,
                Description = description
            };

            await _context.TripTransaction.AddAsync(tripTransaction);

            // Add point to current wallet
            currentWallet.Point += newPoint;

            // Update biker total point
            biker.TotalPoint += newPoint;
            biker.MaxTotalPoint += newPoint;

            // Save change to 4 tables: feedback, tripTransaction, wallet, user
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                _logger.LogInformation("Failed to create new trip transaction");
                throw new Exception("Failed to create new trip transaction.");
            }

            _logger.LogInformation("Successfully created trip transaction");

            var tripTransactionPoint = tripTransaction.AmountOfPoint;
            int userGotPointUpdatedId;
            int userTotalPoint;

            if (tripTransaction.Description.Equals(Constant.TripTipPoint))
            {
                var keer = await _context.User.Where(u => u.UserId == trip.KeerId)
                    .Where(u => u.IsDeleted == false)
                    .SingleOrDefaultAsync();

                if (keer == null)
                {
                    _logger.LogInformation("User with UserId {UserId} doesn't exist", trip.KeerId);
                    throw new Exception($"User with UserId {trip.KeerId} doesn't exist.");
                }

                // Create point history for both keer and biker
                userGotPointUpdatedId = biker.UserId;
                userTotalPoint = biker.TotalPoint;
                
                await _pointHistoryCreation.Run(userGotPointUpdatedId, 
                    (int) HistoryType.TripTransaction,
                    tripTransaction.TripTransactionId, 
                    tripTransactionPoint, userTotalPoint, 
                    tripTransaction.Description,
                    tripTransaction.TransactionDate);
                
                userGotPointUpdatedId = keer.UserId;
                userTotalPoint = keer.TotalPoint;
                // For Keer, point must be subtracted, so tripTransactionPoint must be negative number
                tripTransactionPoint *= -1;
                
                await _pointHistoryCreation.Run(userGotPointUpdatedId, 
                    (int) HistoryType.TripTransaction,
                    tripTransaction.TripTransactionId, 
                    tripTransactionPoint, userTotalPoint, 
                    tripTransaction.Description,
                    tripTransaction.TransactionDate);
            }
            else
            {
                userGotPointUpdatedId = biker.UserId;
                userTotalPoint = biker.TotalPoint;
                
                // Create point history for biker only
                await _pointHistoryCreation.Run(userGotPointUpdatedId, 
                    (int) HistoryType.TripTransaction,
                    tripTransaction.TripTransactionId, 
                    tripTransactionPoint, userTotalPoint, 
                    tripTransaction.Description,
                    tripTransaction.TransactionDate);
            }
        }
    }
}