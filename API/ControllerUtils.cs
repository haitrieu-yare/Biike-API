using System;
using Microsoft.AspNetCore.Http;
using Domain.Enums;

namespace API
{
	public static class ControllerUtils
	{
		public static int GetRole(HttpContext httpContext)
		{
			int role = 0;
			foreach (RoleStatus roleStatus in Enum.GetValues(typeof(RoleStatus)))
			{
				if (httpContext.User.IsInRole(((int)roleStatus).ToString()))
				{
					role = (int)roleStatus;
				}
			}
			return role;
		}
		public static ValidationDTO Validate(HttpContext httpContext)
		{
			// Mục đích sử dụng
			// 1) Kiểm tra user gửi request có tồn tại hay không
			// 2) Kiểm tra user này có phải là admin hay không
			// 3) Lấy Id của user tạo request này

			#region Check user make request existence
			var userRequestIdClaim = httpContext.User.FindFirst(c => c.Type.Equals("user_id"));
			string? userRequestIdString = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestIdString))
				return new ValidationDTO
				{
					IsUserFound = false
				};
			#endregion

			#region Check Admin
			bool isAdmin = httpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			#endregion

			#region Get requester's Id
			int userRequestId = int.Parse(userRequestIdString);
			#endregion

			return new ValidationDTO
			{
				IsUserFound = true,
				IsAdmin = isAdmin,
				IsAuthorized = true,
				UserRequestId = userRequestId
			};
		}
		public static ValidationDTO Validate(HttpContext httpContext, int? userIdGetRequested)
		{
			// Mục đích sử dụng
			// 1) Kiểm tra user được request có tồn tại hay không
			// 2) Kiểm tra user gửi request có tồn tại hay không
			// 2) Kiểm tra user này có phải là admin hay không
			// 3) Lấy Id của user tạo request này

			#region Check requested user existence
			if (userIdGetRequested == null)
				return new ValidationDTO
				{
					IsUserFound = false
				};
			#endregion

			#region Check user make request existence
			var userRequestIdClaim = httpContext.User.FindFirst(c => c.Type.Equals("user_id"));
			string? userRequestIdString = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDTO
			{
				IsUserFound = false
			};
			#endregion

			#region Check Admin
			bool isAdmin = httpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			#endregion

			#region Get requester's Id
			int userRequestId = int.Parse(userRequestIdString);
			#endregion

			// Nếu người request và người được request không trùng nhau và cũng không phai là admin thì trả BadRequest
			if (userRequestId != userIdGetRequested && !isAdmin) return new ValidationDTO
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