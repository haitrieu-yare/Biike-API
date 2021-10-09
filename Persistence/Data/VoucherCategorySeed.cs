using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class VoucherCategorySeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.VoucherCategory.Any()) return;

            DateTime createdDate = DateTime.Parse("2021/09/01");

            var voucherCategories = new List<VoucherCategory>
            {
                new()
                {
                    CategoryName = "Bình Dân",
                    CreatedDate = createdDate
                },
                new()
                {
                    CategoryName = "Cao Cấp",
                    CreatedDate = createdDate
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var voucherCategory in voucherCategories)
            {
                await context.VoucherCategory.AddAsync(voucherCategory);
                await context.SaveChangesAsync();
            }
        }
    }
}