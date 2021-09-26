using System;

namespace Application.Users.DTOs
{
	public class UserProfileEditDTO
	{
		public string? UserFullname { get; set; }
		public string? Avatar { get; set; }
		public int? Gender { get; set; }
		public DateTime? BirthDate { get; set; }
	}
}