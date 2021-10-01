using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
	public class UserExistDTO
	{
		[Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? Email { get; set; }
	}
}