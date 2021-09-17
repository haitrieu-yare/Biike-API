using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Enums;

namespace Persistence.Data
{
	public static class TripSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Trip.Any()) return;

			var trips = new List<Trip>
			{
				new Trip
				{
					KeerId = 1,
					BikerId = 4,
					RouteId = 1,
					BookTime = DateTime.Now.AddDays(-10),
					PickupTime = DateTime.Now.AddDays(-9),
					FinishedTime = DateTime.Now.AddDays(-9).AddMilliseconds(1800000),
					Status = (int) TripStatus.Completed,
					PlateNumber = "7000",
					IsScheduled = true,
				},
				new Trip
				{
					KeerId = 2,
					BikerId = 5,
					RouteId = 2,
					BookTime = DateTime.Now.AddDays(-5),
					PickupTime = DateTime.Now.AddDays(-5).AddMilliseconds(300000),
					FinishedTime = DateTime.Now.AddDays(-5).AddMilliseconds(1200000),
					Status = (int) TripStatus.Completed,
					PlateNumber = "7001",
					IsScheduled = false,
				},
				new Trip
				{
					KeerId = 6,
					BikerId = 7,
					RouteId = 3,
					BookTime = DateTime.Now.AddDays(-2),
					PickupTime = DateTime.Now.AddDays(-2).AddMilliseconds(600000),
					FinishedTime = DateTime.Now.AddDays(-2).AddMilliseconds(900000),
					Status = (int) TripStatus.Completed,
					PlateNumber = "7004",
					IsScheduled = false,
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var trip in trips)
			{
				await context.Trip.AddAsync(trip);
				await context.SaveChangesAsync();
			}
		}
	}
}