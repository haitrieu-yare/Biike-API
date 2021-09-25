using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
	public static class RouteSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Route.Any()) return;

			var routes = new List<Route>
			{
				new Route
				{
					DepartureId = 1,
					DestinationId = 2,
					DefaultPoint = 10,
				},
				new Route
				{
					DepartureId = 2,
					DestinationId = 1,
					DefaultPoint = 10,
				},
				new Route
				{
					DepartureId = 1,
					DestinationId = 3,
					DefaultPoint = 15,
				},
				new Route
				{
					DepartureId = 3,
					DestinationId = 1,
					DefaultPoint = 15,
				},
				new Route
				{
					DepartureId = 1,
					DestinationId = 4,
					DefaultPoint = 12,
				},
				new Route
				{
					DepartureId = 4,
					DestinationId = 1,
					DefaultPoint = 12,
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var route in routes)
			{
				await context.Route.AddAsync(route);
				await context.SaveChangesAsync();
			}
		}
	}
}