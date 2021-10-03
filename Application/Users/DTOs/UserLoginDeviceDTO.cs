
using System;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Application.Users.DTOs
{
	public class UserLoginDeviceDTO
	{
		[Required]
		public string? LastLoginDevice { get; set; }

		[Required]
		public DateTime? LastTimeLogin { get; set; }
		public DateTime? LastTimeRefresh { get; set; }
	}
}