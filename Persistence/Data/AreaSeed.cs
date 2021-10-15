using System;
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

            var createdDate = DateTime.Parse("2021/09/01");

            var areas = new List<Area>
            {
                new()
                {
                    Name = "Đại Học FPT TP.HCM",
                    CreatedDate = createdDate
                }
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