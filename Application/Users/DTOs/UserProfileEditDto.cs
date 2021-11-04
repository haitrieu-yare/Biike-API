using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserProfileEditDto
    {
        private readonly string? _userFullname;
        private readonly string? _avatar;

        public string? UserFullname
        {
            get => _userFullname;
            init => _userFullname = value?.Trim();
        }

        public string? Avatar
        {
            get => _avatar;
            init => _avatar = value?.Trim();
        }

        public int? Gender { get; init; }
        public DateTime? BirthDate { get; init; }
    }
}