using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class WalletSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Wallet.Any()) return;

            var fromDate = DateTime.Parse("2021/09/01");
            var toDate = DateTime.Parse("2021/12/31 23:59:59.9999999");

            var wallets = new List<Wallet>
            {
                new()
                {
                    UserId = 1,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                },
                new()
                {
                    UserId = 2,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                },
                new()
                {
                    UserId = 3,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                },
                new()
                {
                    UserId = 4,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                },
                new()
                {
                    UserId = 7,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                },
                new()
                {
                    UserId = 8,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Point = 5000
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var wallet in wallets)
            {
                await context.Wallet.AddAsync(wallet);
                await context.SaveChangesAsync();
            }
        }
    }
}