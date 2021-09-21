using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence.Data
{
	public static class StationSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Station.Any()) return;

			var stations = new List<Station>
			{
				new Station
				{
					AreaId = 1,
					Name = "Đại Học FPT",
					Address = "Lô E2a-7, Đường D1, Khu Công Nghệ Cao, Long Thạnh Mỹ, Hồ Chí Minh",
					Coordinate = "10.8414899,106.8078577",
					IsDeleted = false,
				},
				new Station
				{
					AreaId = 1,
					Name = "Cổng Khu Công Nghệ Cao",
					Address = "900 Xa lộ Hà Nội, Khu Phố 6, Quận 9, Hồ Chí Minh",
					Coordinate = "10.857506,106.7872426",
					IsDeleted = false,
				},
				new Station
				{
					AreaId = 1,
					Name = "Chung Cư Sky 9",
					Address = "61 Đường Số 1, Khu Phố 2, Quận 9, Hồ Chí Minh",
					Coordinate = "10.8044973,106.7890194",
					IsDeleted = false,
				},
				new Station
				{
					AreaId = 1,
					Name = "Ngã Tư Thủ Đức",
					Address = "1A Lê Văn Việt, Hiệp Phú, Thành Phố Thủ Đức, Hồ Chí Minh",
					Coordinate = "10.8488652,106.773979",
					IsDeleted = false,
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var station in stations)
			{
				await context.Station.AddAsync(station);
				await context.SaveChangesAsync();
			}
		}
	}
}