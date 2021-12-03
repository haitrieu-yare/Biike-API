using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Data
{
    public static class VoucherSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Voucher.Any()) return;

            var createdDate = DateTime.Parse("2021/09/01");

            var vouchers = new List<Voucher>
            {
                new()
                {
                    VoucherCategoryId = 1,
                    VoucherName = "PASSIO20K",
                    Brand = "Passio",
                    StartDate = DateTime.Parse("2021/09/01"),
                    EndDate = DateTime.Parse("2021/12/31 23:59:59.9999999"),
                    Quantity = 100,
                    Remaining = 100,
                    AmountOfPoint = 200,
                    Description = "Voucher giảm giá đồ uống Passio 20 ngàn",
                    TermsAndConditions = "Áp dụng cho đơn hàng có giá trị từ 30 ngàn trở lên",
                    CreatedDate = createdDate
                },
                new()
                {
                    VoucherCategoryId = 1,
                    VoucherName = "ELEVENSEVEN30K",
                    Brand = "ElevenSeven",
                    StartDate = DateTime.Parse("2021/09/01"),
                    EndDate = DateTime.Parse("2021/12/30 23:59:59.9999999"),
                    Quantity = 150,
                    Remaining = 150,
                    AmountOfPoint = 300,
                    Description = "Voucher giảm giá 30 ngàn khi mua hàng ở Eleven Seven",
                    TermsAndConditions = "Áp dụng cho đơn hàng có giá trị từ 40 ngàn trở lên",
                    CreatedDate = createdDate
                },
                new()
                {
                    VoucherCategoryId = 2,
                    VoucherName = "PASSIO50K",
                    Brand = "Passio",
                    StartDate = DateTime.Parse("2021/09/01"),
                    EndDate = DateTime.Parse("2021/12/29 23:59:59.9999999"),
                    Quantity = 50,
                    Remaining = 50,
                    AmountOfPoint = 500,
                    Description = "Voucher giảm giá 50 ngàn khi mua sản phẩm ở Passio",
                    TermsAndConditions = "Áp dụng cho đơn hàng có giá trị từ 50 ngàn trở lên",
                    CreatedDate = createdDate
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var voucher in vouchers)
            {
                await context.Voucher.AddAsync(voucher);
                await context.SaveChangesAsync();
            }
        }
    }
}