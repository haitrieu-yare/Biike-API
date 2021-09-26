using System;

namespace Application.Users.DTOs
{
	public class UserProfileDTO
	{
		public int? UserId { get; set; }
		public string? UserPhoneNumber { get; set; }
		public string? UserFullname { get; set; }
		public string? Avatar { get; set; }
		public int? Gender { get; set; }
		public double? UserStar { get; set; }
		public DateTime? BirthDate { get; set; }
		public string? AreaName { get; set; } = "Đại học FPT TP.HCM";
	}
}