using System;

namespace Application.AppUsers.DTOs
{
	public class AppUserProfileDTO
	{
		public int UserId { get; set; }
		public string UserPhoneNumber { get; set; }
		public string UserEmail { get; set; }
		public string UserFullname { get; set; }
		public string Avatar { get; set; }
		public int Gender { get; set; }
		public double UserStar { get; set; }
		public int UserStatus { get; set; }
		public string LastLoginDevice { get; set; }
		public DateTime? LastTimeLogin { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsBikeVerified { get; set; }
		public string AreaName { get; set; } = "Đại học FPT TP.HCM";
	}
}