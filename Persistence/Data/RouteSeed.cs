using System;
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

            var createdDate = DateTime.Parse("2021/09/01");

            var routes = new List<Route>
            {
                new()
                {
                    DepartureId = 1,
                    DestinationId = 2,
                    DefaultPoint = 6,
                    Distance = 3.1,
                    AreaId = 1,
                    CreatedDate = createdDate
                },
                new()
                {
                    DepartureId = 2,
                    DestinationId = 1,
                    DefaultPoint = 6,
                    Distance = 3.1,
                    AreaId = 1,
                    CreatedDate = createdDate
                },
                new()
                {
                    DepartureId = 1,
                    DestinationId = 3,
                    DefaultPoint = 11,
                    Distance = 5.4,
                    AreaId = 1,
                    CreatedDate = createdDate
                },
                new()
                {
                    DepartureId = 3,
                    DestinationId = 1,
                    DefaultPoint = 11,
                    Distance = 5.4,
                    AreaId = 1,
                    CreatedDate = createdDate
                },
                new()
                {
                    DepartureId = 1,
                    DestinationId = 4,
                    DefaultPoint = 12,
                    Distance = 5.8,
                    AreaId = 1,
                    CreatedDate = createdDate
                },
                new()
                {
                    DepartureId = 4,
                    DestinationId = 1,
                    DefaultPoint = 12,
                    Distance = 5.8,
                    AreaId = 1,
                    CreatedDate = createdDate
                }
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