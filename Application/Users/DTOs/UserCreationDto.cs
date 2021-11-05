using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserCreationDto
    {
        [Required]
        [RegularExpression(@"^(\+84)([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; init; }

        private readonly string? _email;

        [Required]
        [EmailAddress]
        [RegularExpression(@".*@fpt\.edu\.vn$", ErrorMessage = "Must use fpt email.")]
        public string? Email
        {
            get => _email;
            init => _email = value?.Trim();
        }

        [Required] [MinLength(6)] [MaxLength(32)] public string? Password { get; init; }

        private readonly string? _fullname;

        [Required]
        public string? Fullname
        {
            get => _fullname;
            init => _fullname = value?.Trim();
        }
    }
}