using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Persistence.Data
{
    public static class TripTransactionSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.TripTransaction.Any()) return;

            DateTime currentTime = CurrentTime.GetCurrentTime();

            var tripTransactions = new List<TripTransaction>
            {
                new()
                {
                    TripId = 1,
                    WalletId = 3,
                    AmountOfPoint = 10,
                    TransactionDate = currentTime.AddDays(-9).AddSeconds(600)
                },
                new()
                {
                    TripId = 1,
                    WalletId = 3,
                    AmountOfPoint = 10,
                    TransactionDate = currentTime.AddDays(-9).AddSeconds(900)
                },
                new()
                {
                    TripId = 2,
                    WalletId = 4,
                    AmountOfPoint = 12,
                    TransactionDate = currentTime.AddDays(-2).AddSeconds(420 + 600)
                },
                new()
                {
                    TripId = 2,
                    WalletId = 4,
                    AmountOfPoint = 10,
                    TransactionDate = currentTime.AddDays(-2).AddSeconds(420 + 600 + 300)
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var tripTransaction in tripTransactions)
            {
                await context.TripTransaction.AddAsync(tripTransaction);
                await context.SaveChangesAsync();
            }
        }
    }
}