using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
	public class UserCreateDTO
	{
		[Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? Email { get; set; }

		[Required]
		public string? Password { get; set; }

		[Required]
		public string? Fullname { get; set; }
	}
}