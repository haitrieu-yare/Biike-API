using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence.Data
{
	public static class IntimacySeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Intimacy.Any()) return;

			var intimacies = new List<Intimacy>
			{
				new Intimacy
				{
					UserOneId = 5,
					UserTwoId = 6,
					IsBlock = true,
					BlockTime = DateTime.Now.AddDays(-1),
				},
				new Intimacy
				{
					UserOneId = 6,
					UserTwoId = 5,
					IsBlock = true,
					BlockTime = DateTime.Now.AddDays(-1).AddMilliseconds(1500000),
				},
				new Intimacy
				{
					UserOneId = 7,
					UserTwoId = 2,
					IsBlock = false,
					BlockTime = DateTime.Now.AddDays(-3),
					UnblockTime = DateTime.Now.AddDays(-2),
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var intimacy in intimacies)
			{
				await context.Intimacy.AddAsync(intimacy);
				await context.SaveChangesAsync();
			}
		}
	}
}