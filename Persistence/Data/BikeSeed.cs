using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
	public static class BikeSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Bike.Any()) return;

			var bikes = new List<Bike>
			{
				new Bike
				{
					UserId = 4,
					PlateNumber = "7000",
					Color = "Gold",
					Brand = "Honda",
				},
				new Bike
				{
					UserId = 5,
					PlateNumber = "7001",
					Color = "Blue",
					Brand = "Yamaha",
				},
				new Bike
				{
					UserId = 6,
					PlateNumber = "7002",
					Color = "Red",
					Brand = "Honda",
				},
				new Bike
				{
					UserId = 7,
					PlateNumber = "7004",
					Color = "Green",
					Brand = "Suzuki",
				},
				new Bike
				{
					UserId = 8,
					PlateNumber = "7005",
					Color = "Black",
					Brand = "Yamaha",
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var bike in bikes)
			{
				await context.Bike.AddAsync(bike);
				await context.SaveChangesAsync();
			}
		}
	}
}