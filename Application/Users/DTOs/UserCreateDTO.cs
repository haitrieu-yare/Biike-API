using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
	public class UserCreateDTO
	{
		[Required]
		[RegularExpression(@"^(\+84)([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
		public string? PhoneNumber { get; set; }

		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[MinLength(6)]
		public string? Password { get; set; }

		[Required]
		public string? Fullname { get; set; }
	}
}