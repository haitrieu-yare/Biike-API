using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
	public class UserExistDto
	{
		[Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? Email { get; set; }
	}
}