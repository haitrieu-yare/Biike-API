using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Persistence.Data
{
    public static class RedemptionSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Redemption.Any()) return;

            DateTime currentTime = CurrentTime.GetCurrentTime();

            var redemptions = new List<Redemption>
            {
                new()
                {
                    WalletId = 1,
                    VoucherId = 1,
                    VoucherCode = "ORANGEAUTUMN20",
                    VoucherPoint = 200,
                    IsUsed = false,
                    RedemptionDate = currentTime.AddDays(-5)
                },
                new()
                {
                    WalletId = 3,
                    VoucherId = 2,
                    VoucherCode = "BLUESKY30",
                    VoucherPoint = 300,
                    IsUsed = false,
                    RedemptionDate = currentTime.AddDays(-3)
                },
                new()
                {
                    WalletId = 4,
                    VoucherId = 3,
                    VoucherCode = "GOLDMEDAL50",
                    VoucherPoint = 500,
                    IsUsed = true,
                    RedemptionDate = currentTime.AddDays(-2)
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var redemption in redemptions)
            {
                await context.Redemption.AddAsync(redemption);
                await context.SaveChangesAsync();
            }
        }
    }
}