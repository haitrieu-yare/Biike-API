using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class ConfigurationSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Configuration.Any()) return;

            var createdDate = DateTime.Parse("2021/09/01");

            var configurations = new List<Configuration>
            {
                new()
                {
                    ConfigurationName = "ConversionRate", 
                    ConfigurationValue = "0.06",
                    UserId = 6, 
                    CreatedDate = createdDate
                },
                new()
                {
                    ConfigurationName = "ActiveRadius", 
                    ConfigurationValue = "10",
                    UserId = 6, 
                    CreatedDate = createdDate
                },
                new()
                {
                    ConfigurationName = "FuelConversion", 
                    ConfigurationValue = "0.025",
                    UserId = 6, 
                    CreatedDate = createdDate
                },
                new()
                {
                    ConfigurationName = "IsCancellationLimitOn", 
                    ConfigurationValue = "true",
                    UserId = 6, 
                    CreatedDate = createdDate
                },
                new()
                {
                    ConfigurationName = "CancellationLimit", 
                    ConfigurationValue = "5",
                    UserId = 6, 
                    CreatedDate = createdDate
                },
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var configuration in configurations)
            {
                await context.Configuration.AddAsync(configuration);
                await context.SaveChangesAsync();
            }
        }
    }
}