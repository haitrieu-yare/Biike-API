using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Persistence.Data
{
    public static class IntimacySeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Intimacy.Any()) return;

            var currentTime = CurrentTime.GetCurrentTime();

            var intimacies = new List<Intimacy>
            {
                new()
                {
                    UserOneId = 7,
                    UserTwoId = 8,
                    IsBlock = true,
                    UserName = "Nguyễn Minh Tường",
                    BlockTime = currentTime.AddDays(-1)
                },
                new()
                {
                    UserOneId = 8,
                    UserTwoId = 7,
                    IsBlock = false,
                    UserName = "Trương Minh Phú",
                    BlockTime = currentTime.AddDays(-3),
                    UnblockTime = currentTime.AddDays(-2)
                }
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