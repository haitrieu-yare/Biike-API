using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserProfileEditDto
    {
        public string? UserFullname { get; init; }
        public string? Avatar { get; init; }
        public int? Gender { get; init; }
        public DateTime? BirthDate { get; init; }
    }
}