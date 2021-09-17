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
					Gender = 1,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335001",
					Email = "thanhtam@gmail.com",
					Gender = 0,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335002",
					Email = "phuonguyen@gmail.com",
					Gender = 0,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335003",
					Email = "huuphat@gmail.com",
					Gender = 1,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335004",
					Email = "thaovan@gmail.com",
					Gender = 0,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335005",
					Email = "dangkhoa@gmail.com",
					Gender = 1,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335006",
					Email = "minhphu@gmail.com",
					Gender = 1,
					Status = (int) AppUserStatus.Verified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335007",
					Email = "huonggiang@gmail.com",
					Gender = 0,
					Status = (int) AppUserStatus.Blocked,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = true,
				},
				new AppUser
				{
					PhoneNumber = "0983335008",
					Email = "lamvicon@gmail.com",
					Gender = 1,
					Status = (int) AppUserStatus.Unverified,
					Star = 0,
					CreatedDate = DateTime.Now,
					IsBikeVerified = false,
				},
				new AppUser
				{
					PhoneNumber = "0983335009",
					Email = "giahung@gmail.com",
					Gender = 1,
					Status = (int) AppUserStatus.Deleted,
					Star = 0,
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