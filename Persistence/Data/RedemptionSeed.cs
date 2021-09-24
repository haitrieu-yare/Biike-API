using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
	public static class RedemptionSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Redemption.Any()) return;

			var redemptions = new List<Redemption>
			{
				new Redemption
				{
					WalletId = 4,
					VoucherId = 1,
					VourcherCode = "ORANGEAUTUMN20",
					IsUsed = false,
					RedemptionDate = DateTime.Now,
				},
				new Redemption
				{
					WalletId = 5,
					VoucherId = 2,
					VourcherCode = "BLUESKY30",
					IsUsed = false,
					RedemptionDate = DateTime.Now,
				},
				new Redemption
				{
					WalletId = 6,
					VoucherId = 3,
					VourcherCode = "GOLDMEDAL50",
					IsUsed = true,
					RedemptionDate = DateTime.Now,
				},
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