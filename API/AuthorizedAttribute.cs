using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace API
{
	public class AuthorizedAttribute : AuthorizeAttribute
	{
		public AuthorizedAttribute(params RoleStatus[] roles)
		{
			// Get all RoleStatus params and turn into List<int>
			// with int is the result when convert Enum to int
			List<int> roleStatusInt = roles.Select(role => (int) role).ToList();

			// Turn List<int> into a string
			Roles = string.Join(",", roleStatusInt);
		}
	}
}