using System.Text.Json.Serialization;

namespace Application.Users.DTOs
{
	public class UserActivationDto
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? IsEmailVerified { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? IsPhoneVerified { get; set; }

		public bool? IsVerified { get; set; }
	}
}