
using System;
using Domain;

namespace Application.Users.DTOs
{
	public class UserLoginDeviceDTO
	{
		public string LastLoginDevice { get; set; } = string.Empty;
		public DateTime LastTimeLogin { get; set; } = CurrentTime.GetCurrentTime();
	}
}