using System;
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
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 200,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 2,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 250,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 3,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 150,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 4,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 100,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 5,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 300,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 6,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 350,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 7,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 400,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 8,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 30,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 9,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 10,
					Status = 2,
				},
				new Wallet
				{
					AppUserId = 10,
					FromDate = DateTime.Parse("2021/09/01 00:00:00"),
					ToDate = DateTime.Parse("2021/12/31 23:59:59"),
					Point = 90,
					Status = 2,
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