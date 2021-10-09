using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Domain.Enums;

namespace API
{
	public static class ControllerUtils
	{
		public static int GetRole(HttpContext httpContext)
		{
			var role = 0;
			foreach (RoleStatus roleStatus in Enum.GetValues(typeof(RoleStatus)))
			{
				if (httpContext.User.IsInRole(((int)roleStatus).ToString()))
				{
					role = (int)roleStatus;
				}
			}

			return role;
		}

		public static ValidationDto Validate(HttpContext httpContext)
		{
			#region Check if the userId of the user sent the request is null

			Claim? userRequestIdClaim = httpContext.User.FindFirst(claim => claim.Type.Equals("user_id"));
			string? userRequestIdString = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDto { IsUserFound = false };

			#endregion

			#region Get userId of the user sent the request

			int userRequestId = int.Parse(userRequestIdString);

			#endregion

			#region Check if the user sent the request is admin

			bool isAdmin = httpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());

			#endregion

			return new ValidationDto
			{
				IsUserFound = true,
				IsAdmin = isAdmin,
				IsAuthorized = true,
				UserRequestId = userRequestId,
			};
		}

		public static ValidationDto Validate(HttpContext httpContext, int? userRequestedId)
		{
			#region Check if the userId of the user that get requested is null

			if (userRequestedId == null) return new ValidationDto { IsUserFound = false };

			#endregion

			#region Check if the userId of the user sent the request is null

			Claim? userRequestIdClaim = httpContext.User.FindFirst(claim => claim.Type.Equals("user_id"));
			string? userRequestIdString = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDto { IsUserFound = false };

			#endregion

			#region Check Admin

			bool isAdmin = httpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());

			#endregion

			#region Get requester's Id

			int userRequestId = int.Parse(userRequestIdString);

			#endregion

			// Nếu người request và người được request không trùng nhau và cũng không phai là admin thì trả BadRequest
			if (userRequestId != userRequestedId && !isAdmin)
				return new ValidationDto
				{
					IsUserFound = true,
					IsAdmin = isAdmin,
					IsAuthorized = false,
					UserRequestId = userRequestId,
				};

			return new ValidationDto
			{
				IsUserFound = true,
				IsAdmin = isAdmin,
				IsAuthorized = true,
				UserRequestId = userRequestId,
			};
		}
	}

	public class ValidationDto
	{
		public bool IsUserFound { get; init; }
		public bool IsAdmin { get; init; }
		public bool IsAuthorized { get; init; }
		public int UserRequestId { get; init; }
	}
}