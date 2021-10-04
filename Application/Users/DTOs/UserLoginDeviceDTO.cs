using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
	public class UserLoginDeviceDTO
	{
		[Required]
		public string? LastLoginDevice { get; set; }
		public DateTime? LastTimeLogin { get; set; }
	}
}