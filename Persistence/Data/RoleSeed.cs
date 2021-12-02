using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class RoleSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Role.Any()) return;

            var roles = new List<Role>
            {
                new()
                {
                    RoleName = "Keer"
                },
                new()
                {
                    RoleName = "Biker"
                },
                new()
                {
                    RoleName = "Admin"
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var role in roles)
            {
                await context.Role.AddAsync(role);
                await context.SaveChangesAsync();
            }
        }
    }
}