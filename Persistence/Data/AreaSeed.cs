using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
	public static class AreaSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Area.Any()) return;

			var areas = new List<Area>
			{
				new Area
				{
					Name = "Đại học FPT TP.HCM",
					IsDeleted = false,
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var area in areas)
			{
				await context.Area.AddAsync(area);
				await context.SaveChangesAsync();
			}
		}
	}
}