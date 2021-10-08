using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Persistence.Data
{
	public static class UserSeed
	{
		public static async Task<int> SeedData(DataContext context)
		{
			if (context.User.Any()) return 0;

			DateTime createdDate = DateTime.Parse("2021/09/01");

			var users = new List<User>
			{
				new User
				{
					PhoneNumber = "+84983335000",
					Email = "haitrieu@fpt.edu.vn",
					FullName = "Đinh Phan Hải Triều",
					Avatar = "https://ui-avatars.com/api/?name=Hai+Trieu&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Male,
					Status = (int)UserStatus.Active,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335001",
					Email = "thanhtam@fpt.edu.vn",
					FullName = "Nguyễn Thế Thanh Tâm",
					Avatar = "https://ui-avatars.com/api/?name=Thanh+Tam&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Female,
					Status = (int)UserStatus.Active,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335002",
					Email = "phuonguyen@fpt.edu.vn",
					FullName = "Nguyễn Lê Phương Uyên",
					Avatar = "https://ui-avatars.com/api/?name=Phuong+Uyen&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Female,
					Status = (int)UserStatus.Active,
					Role = (int) RoleStatus.Biker,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					IsBikeVerified = true,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335003",
					Email = "huuphat@fpt.edu.vn",
					FullName = "Đỗ Hữu Phát",
					Avatar = "https://ui-avatars.com/api/?name=Huu+Phat&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Male,
					Status = (int)UserStatus.Active,
					Role = (int) RoleStatus.Biker,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					IsBikeVerified = true,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335004",
					Email = "thaovan@fpt.edu.vn",
					FullName = "Lê Ngọc Thảo Vân",
					Avatar = "https://ui-avatars.com/api/?name=Thao+Van&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Female,
					Status = (int)UserStatus.Active,
					Role = (int) RoleStatus.Admin,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335005",
					Email = "dangkhoa@fpt.edu.vn",
					FullName = "Nguyễn Đăng Khoa",
					Avatar = "https://ui-avatars.com/api/?name=Dang+Khoa&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Male,
					Status = (int)UserStatus.Active,
					Role = (int) RoleStatus.Admin,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335006",
					Email = "minhphu@fpt.edu.vn",
					FullName = "Trương Minh Phú",
					Avatar = "https://ui-avatars.com/api/?name=Minh+Phu&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Male,
					Status = (int)UserStatus.Active,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					CreatedDate = createdDate,
				},
				new User
				{
					PhoneNumber = "+84983335007",
					Email = "minhtuong@fpt.edu.vn",
					FullName = "Nguyễn Minh Tường",
					Avatar = "https://ui-avatars.com/api/?name=Minh+Tuong&background=random&rounded=true&size=128",
					Gender = (int) GenderStatus.Male,
					Status = (int)UserStatus.Active,
					Role = (int) RoleStatus.Biker,
					IsEmailVerified = true,
					IsPhoneVerified = true,
					IsBikeVerified = true,
					CreatedDate = createdDate,
				},
			};

			// Save change for each item because EF doesn't insert like the order
			// we define in our list.
			foreach (var user in users)
			{
				await context.User.AddAsync(user);
				await context.SaveChangesAsync();
			}
			return 1;
		}
	}
}