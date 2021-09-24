using System;

namespace Application.AppUsers.DTOs
{
	public class AppUserSelfProfileDTO
	{
		public int UserId { get; set; }
		public string UserPhoneNumber { get; set; }
		public string UserEmail { get; set; }
		public string UserFullname { get; set; }
		public string Avatar { get; set; }
		public int Gender { get; set; }
		public double UserStar { get; set; }
		public int TotalPoint { get; set; }
		public DateTime BirthDate { get; set; }
		public string AreaName { get; set; } = "Đại học FPT TP.HCM";
	}
}