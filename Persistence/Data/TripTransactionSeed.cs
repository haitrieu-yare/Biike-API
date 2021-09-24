using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence.Data
{
	public static class TripTransactionSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.TripTransaction.Any()) return;

			var tripTransactions = new List<TripTransaction>
			{
				new TripTransaction
				{
					TripId = 1,
					WalletId = 4,
					AmountOfPoint = 10,
					TransactionDate = DateTime.Now.AddDays(-9).AddMilliseconds(1800000),
				},
				new TripTransaction
				{
					TripId = 2,
					WalletId = 5,
					AmountOfPoint = 15,
					TransactionDate = DateTime.Now.AddDays(-5).AddMilliseconds(1200000),
				},
				new TripTransaction
				{
					TripId = 3,
					WalletId = 7,
					AmountOfPoint = 12,
					TransactionDate = DateTime.Now.AddDays(-2).AddMilliseconds(900000),
				},
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