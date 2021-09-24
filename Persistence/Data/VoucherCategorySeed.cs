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

			var voucherCategories = new List<VoucherCategory>
			{
				new VoucherCategory
				{
					CategoryName = "Bình Dân",
				},
				new VoucherCategory
				{
					CategoryName = "Cao Cấp",
				},
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