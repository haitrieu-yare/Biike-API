using System;
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

            var createdDate = DateTime.Parse("2021/09/01");

            var bikes = new List<Bike>
            {
                new()
                {
                    UserId = 3,
                    PlateNumber = "7000",
                    BikeOwner = "Phương Uyên",
                    Picture = "thisispicturelink",
                    Color = "Gold",
                    Brand = "Honda",
                    CreatedDate = createdDate
                },
                new()
                {
                    UserId = 4,
                    PlateNumber = "7001",
                    BikeOwner = "Hữu Phát",
                    Picture = "thisispicturelink",
                    Color = "Blue",
                    Brand = "Yamaha",
                    CreatedDate = createdDate
                },
                new()
                {
                    UserId = 8,
                    PlateNumber = "7002",
                    BikeOwner = "Minh Tường",
                    Picture = "thisispicturelink",
                    Color = "Black",
                    Brand = "Suzuki",
                    CreatedDate = createdDate
                }
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