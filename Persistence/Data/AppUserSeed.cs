using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Enums;

namespace Persistence.Data
{
	public static class AppUserSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.AppUser.Any()) return;

			var appusers = new List<AppUser>
			{
				new AppUser
				{
					PhoneNumber = "0983335000",
					Email = "haitrieu.yare@gmail.com",
					FullName = "Đinh Phan Hải Triều",
					Avatar = "https://ui-avatars.com/api/?name=Hai+Trieu&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int) AppUserStatus.Active,
					Star = 4,
					TotalPoint = 0,
					BirthDate = DateTime.Parse("2000-09-02"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335001",
					Email = "thanhtam@gmail.com",
					FullName = "Nguyễn Thế Thanh Tâm",
					Avatar = "https://ui-avatars.com/api/?name=Thanh+Tam&background=random&rounded=true&size=128",
					Gender = 0,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					TotalPoint = 0,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335002",
					Email = "phuonguyen@gmail.com",
					FullName = "Nguyễn Lê Phương Uyên",
					Avatar = "https://ui-avatars.com/api/?name=Phuong+Uyen&background=random&rounded=true&size=128",
					Gender = 0,
					Status = (int) AppUserStatus.Active,
					Star = 4,
					TotalPoint = 0,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335003",
					Email = "huuphat@gmail.com",
					FullName = "Đỗ Hữu Phát",
					Avatar = "https://ui-avatars.com/api/?name=Huu+Phat&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int) AppUserStatus.Active,
					Star = 4,
					TotalPoint = 10,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335004",
					Email = "thaovan@gmail.com",
					FullName = "Lê Ngọc Thảo Vân",
					Avatar = "https://ui-avatars.com/api/?name=Thao+Van&background=random&rounded=true&size=128",
					Gender = 0,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					TotalPoint = 15,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335005",
					Email = "dangkhoa@gmail.com",
					FullName = "Nguyễn Đăng Khoa",
					Avatar = "https://ui-avatars.com/api/?name=Dang+Khoa&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335006",
					Email = "minhphu@gmail.com",
					FullName = "Trương Minh Phú",
					Avatar = "https://ui-avatars.com/api/?name=Minh+Phu&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					TotalPoint = 12,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335007",
					Email = "huonggiang@gmail.com",
					FullName = "Nguyễn Thị Hương Giang",
					Avatar = "https://ui-avatars.com/api/?name=Huong+Giang&background=random&rounded=true&size=128",
					Gender = 0,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335008",
					Email = "lamvicon@gmail.com",
					FullName = "Lâm Vĩ Côn",
					Avatar = "https://ui-avatars.com/api/?name=Vi+Con&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335009",
					Email = "giahung@gmail.com",
					FullName = "Giang Gia Hưng",
					Avatar = "https://ui-avatars.com/api/?name=Gia+Hung&background=random&rounded=true&size=128",
					Gender = 1,
					Status = (int)AppUserStatus.Active,
					Star = 4,
					BirthDate = DateTime.Parse("2000-05-05"),
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				}
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var appuser in appusers)
			{
				await context.AppUser.AddAsync(appuser);
				await context.SaveChangesAsync();
			}
		}
	}
}