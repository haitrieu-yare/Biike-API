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
                    BikePicture = "randomimage",
                    BikeLicensePicture = "randomimage",
                    PlateNumberPicture = "randomimage",
                    DrivingLicenseFrontPicture = "randomimage",
                    DrivingLicenseBackPicture = "randomimage",
                    Status = 2,
                    Color = "Vàng",
                    Brand = "Honda",
                    BikeType = "Xe số",
                    BikeVolume = "100 - 175 cc",
                    CreatedDate = createdDate
                },
                new()
                {
                    UserId = 4,
                    PlateNumber = "7001",
                    BikeOwner = "Hữu Phát",
                    BikePicture = "randomimage",
                    BikeLicensePicture = "randomimage",
                    PlateNumberPicture = "randomimage",
                    DrivingLicenseFrontPicture = "randomimage",
                    DrivingLicenseBackPicture = "randomimage",
                    Status = 2,
                    Color = "Blue",
                    Brand = "Yamaha",
                    BikeType = "Tay ga",
                    BikeVolume = "100 - 175 cc",
                    CreatedDate = createdDate
                },
                new()
                {
                    UserId = 8,
                    PlateNumber = "7002",
                    BikeOwner = "Minh Tường",
                    BikePicture = "randomimage",
                    BikeLicensePicture = "randomimage",
                    PlateNumberPicture = "randomimage",
                    DrivingLicenseFrontPicture = "randomimage",
                    DrivingLicenseBackPicture = "randomimage",
                    Status = 2,
                    Color = "Black",
                    Brand = "Suzuki",
                    BikeType = "Tay Côn/Moto",
                    BikeVolume = "100 - 175 cc",
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