using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence.Data
{
	public static class WalletSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Wallet.Any()) return;

			var wallets = new List<Wallet>
			{
				new Wallet
				{
					AppUserId = 1,
					Point = 200,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 2,
					Point = 250,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 3,
					Point = 150,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 4,
					Point = 100,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 5,
					Point = 300,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 6,
					Point = 350,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 7,
					Point = 400,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 8,
					Point = 30,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 9,
					Point = 10,
					Status = 1,
				},
				new Wallet
				{
					AppUserId = 10,
					Point = 90,
					Status = 1,
				},
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