using System;

namespace Application.AppUsers.DTOs
{
	public class AppUserInfoDTO
	{
		public int Id { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public string Avatar { get; set; }
		public int Gender { get; set; }
		public int Status { get; set; }
		public string LastLoginDevice { get; set; }
		public DateTime LastTimeLogin { get; set; }
		public double Star { get; set; }
		public DateTime BirthDate { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsBikeVerified { get; set; }
	}
}