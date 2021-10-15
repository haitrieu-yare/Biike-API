using System;

namespace Application.Users.DTOs
{
    public class UserProfileEditDto
    {
        public string? UserFullname { get; set; }
        public string? Avatar { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}