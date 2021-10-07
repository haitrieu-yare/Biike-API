using Microsoft.AspNetCore.Http;
using Domain.Enums;

namespace API
{
	public static class ControllerUtils
	{
		public static ValidationDTO CheckRequestUserId(HttpContext httpContext, int? userId)
		{
			if (userId == null) return new ValidationDTO
			{
				IsUserFound = false
			};

			var userRequestIdClaim = httpContext.User.FindFirst(c => c.Type.Equals("user_id"));
			string? userRequestIdString = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDTO
			{
				IsUserFound = false
			};

			bool isAdmin = httpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			int userRequestId = int.Parse(userRequestIdString);

			if (userRequestId != userId && !isAdmin) return new ValidationDTO
			{
				IsUserFound = true,
				IsAdmin = isAdmin,
				IsAuthorized = false,
				UserRequestId = userRequestId
			};

			return new ValidationDTO
			{
				IsUserFound = true,
				IsAdmin = isAdmin,
				IsAuthorized = true,
				UserRequestId = userRequestId
			};
		}
	}

	public class ValidationDTO
	{
		public bool IsUserFound { get; set; } = false;
		public bool IsAdmin { get; set; } = false;
		public bool IsAuthorized { get; set; } = false;
		public int UserRequestId { get; set; }
	}
}