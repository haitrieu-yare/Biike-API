using System;
using System.Text.Json.Serialization;

namespace Application.Users.DTOs
{
	public class UserDto
	{
		public int? UserId { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Email { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Role { get; set; }
		public string? FullName { get; set; }
		public string? Avatar { get; set; }
		public int? Gender { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? UserStatus { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? LastLoginDevice { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? LastTimeLogin { get; set; }
		public double? UserStar { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? TotalPoint { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? IsBikeVerified { get; set; }
		public DateTime? BirthDate { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? CreatedDate { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? IsDeleted { get; set; }
	}
}