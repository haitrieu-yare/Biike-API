using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class StationSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Station.Any()) return;

            var createdDate = DateTime.Parse("2021/09/01");

            var stations = new List<Station>
            {
                new()
                {
                    AreaId = 1,
                    Name = "Đại Học FPT",
                    Address = "Lô E2a-7, Đường D1, Khu Công Nghệ Cao, Long Thạnh Mỹ, Hồ Chí Minh",
                    Coordinate = "10.8413821,106.8088426",
                    IsCentralPoint = true,
                    CreatedDate = createdDate
                },
                new()
                {
                    AreaId = 1,
                    Name = "Cổng Khu Công Nghệ Cao",
                    Address = "900 Xa lộ Hà Nội, Khu Phố 6, Quận 9, Hồ Chí Minh",
                    Coordinate = "10.857506,106.7872426",
                    IsCentralPoint = false,
                    CreatedDate = createdDate
                },
                new()
                {
                    AreaId = 1,
                    Name = "Chung Cư Sky 9",
                    Address = "61 Đường Số 1, Khu Phố 2, Quận 9, Hồ Chí Minh",
                    Coordinate = "10.8039905,106.7910568",
                    IsCentralPoint = false,
                    CreatedDate = createdDate
                },
                new()
                {
                    AreaId = 1,
                    Name = "Ngã Tư Thủ Đức",
                    Address = "1A Lê Văn Việt, Hiệp Phú, Thành Phố Thủ Đức, Hồ Chí Minh",
                    Coordinate = "10.8492401,106.7738935",
                    IsCentralPoint = false,
                    CreatedDate = createdDate
                }
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