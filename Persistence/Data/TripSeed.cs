using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;

namespace Persistence.Data
{
    public static class TripSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Trip.Any()) return;

            var currentTime = CurrentTime.GetCurrentTime();

            var trips = new List<Trip>
            {
                new()
                {
                    KeerId = 1,
                    BikerId = 3,
                    RouteId = 2,
                    CreatedDate = currentTime.AddDays(-10),
                    BookTime = currentTime.AddDays(-9),
                    PickupTime = currentTime.AddDays(-9).AddSeconds(300),
                    FinishedTime = currentTime.AddDays(-9).AddSeconds(600),
                    Status = (int) TripStatus.Finished,
                    PlateNumber = "7000",
                    IsScheduled = true
                },
                new()
                {
                    KeerId = 1,
                    BikerId = 4,
                    RouteId = 2,
                    CreatedDate = currentTime,
                    BookTime = currentTime.AddDays(2),
                    Status = (int) TripStatus.Waiting,
                    PlateNumber = "7001",
                    IsScheduled = true
                },
                new()
                {
                    KeerId = 1,
                    RouteId = 2,
                    CreatedDate = currentTime,
                    BookTime = currentTime.AddDays(4),
                    Status = (int) TripStatus.Finding,
                    IsScheduled = true
                },
                new()
                {
                    KeerId = 1,
                    BikerId = 8,
                    RouteId = 2,
                    CreatedDate = currentTime.AddDays(-15),
                    BookTime = currentTime.AddDays(-12),
                    Status = (int) TripStatus.Cancelled,
                    CancelPersonId = 1,
                    CancelReason = "Mình bận đột xuất. Sorry!",
                    IsScheduled = true
                },
                new()
                {
                    KeerId = 1,
                    BikerId = 4,
                    RouteId = 2,
                    CreatedDate = currentTime.AddDays(-7),
                    BookTime = currentTime.AddDays(-5),
                    Status = (int) TripStatus.Cancelled,
                    CancelPersonId = 4,
                    CancelReason = "Mình bận đột xuất. Sorry!",
                    IsScheduled = true
                },
                new()
                {
                    KeerId = 2,
                    BikerId = 3,
                    RouteId = 5,
                    CreatedDate = currentTime.AddDays(-2),
                    BookTime = currentTime.AddDays(-2),
                    PickupTime = currentTime.AddDays(-2).AddSeconds(420),
                    FinishedTime = currentTime.AddDays(-2).AddSeconds(420 + 600),
                    Status = (int) TripStatus.Finished,
                    PlateNumber = "7000",
                    IsScheduled = false
                },
                new()
                {
                    KeerId = 2,
                    BikerId = 4,
                    RouteId = 6,
                    CreatedDate = currentTime,
                    BookTime = currentTime.AddDays(3),
                    Status = (int) TripStatus.Waiting,
                    PlateNumber = "7001",
                    IsScheduled = true
                }
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