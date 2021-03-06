using System;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace API
{
    public static class ControllerUtils
    {
        public static int GetRole(HttpContext httpContext)
        {
            var role = 0;
            foreach (RoleStatus roleStatus in Enum.GetValues(typeof(RoleStatus)))
                if (httpContext.User.IsInRole(((int) roleStatus).ToString()))
                    role = (int) roleStatus;

            return role;
        }

        public static ValidationDto Validate(HttpContext httpContext)
        {
            #region Check if the userId of the user sent the request is null

            var userRequestIdClaim = httpContext.User.FindFirst(claim => claim.Type.Equals("user_id"));
            var userRequestIdString = userRequestIdClaim?.Value;

            if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDto {IsUserFound = false};

            #endregion

            #region Get userId of the user sent the request

            var userRequestId = int.Parse(userRequestIdString);

            #endregion

            #region Check if the user sent the request is admin

            var isAdmin = httpContext.User.IsInRole(((int) RoleStatus.Admin).ToString());

            #endregion

            return new ValidationDto
            {
                IsUserFound = true, IsAuthorized = true, IsAdmin = isAdmin, UserRequestId = userRequestId
            };
        }

        public static ValidationDto Validate(HttpContext httpContext, int? userRequestedId)
        {
            #region Check if the userId of the user that is requested is null

            if (userRequestedId == null) return new ValidationDto {IsUserFound = false};

            #endregion

            #region Check if the userId of the user sent the request is null

            var userRequestIdClaim = httpContext.User.FindFirst(claim => claim.Type.Equals("user_id"));
            var userRequestIdString = userRequestIdClaim?.Value;

            if (string.IsNullOrEmpty(userRequestIdString)) return new ValidationDto {IsUserFound = false};

            #endregion

            #region Get userId of the user sent the request

            var userRequestId = int.Parse(userRequestIdString);

            #endregion

            #region Check if the user sent the request is admin

            var isAdmin = httpContext.User.IsInRole(((int) RoleStatus.Admin).ToString());

            #endregion

            #region Check if the user sent the request is the same as the user that is requested

            if (userRequestId != userRequestedId && !isAdmin)
                return new ValidationDto
                {
                    IsUserFound = true, IsAuthorized = false, IsAdmin = isAdmin, UserRequestId = userRequestId
                };

            #endregion

            return new ValidationDto
            {
                IsUserFound = true, IsAuthorized = true, IsAdmin = isAdmin, UserRequestId = userRequestId
            };
        }
    }

    public class ValidationDto
    {
        public bool IsUserFound { get; init; }
        public bool IsAuthorized { get; init; }
        public bool IsAdmin { get; init; }
        public int UserRequestId { get; init; }
    }
}