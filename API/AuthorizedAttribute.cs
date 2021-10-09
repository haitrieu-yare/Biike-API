using System.Collections.Generic;
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
            List<int> roleStatusInt = new();
            foreach (var role in roles)
            {
                // Convert enum to int
                var roleInt = (int) role;
                // Add int variable to List<int>
                roleStatusInt.Add(roleInt);
            }

            // Turn List<int> into a string
            Roles = string.Join(",", roleStatusInt);
        }
    }
}