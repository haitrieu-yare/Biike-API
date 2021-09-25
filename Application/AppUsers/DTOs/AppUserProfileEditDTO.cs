using System;

namespace Application.AppUsers.DTOs
{
	public class AppUserProfileEditDTO
	{
		public string UserFullname { get; set; }
		public string Avatar { get; set; }
		public int? Gender { get; set; }
		public DateTime? BirthDate { get; set; }
	}
}