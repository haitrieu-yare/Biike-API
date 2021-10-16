using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserCreationDto
    {
        [Required]
        [RegularExpression(@"^(\+84)([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; init; }

        [Required] [EmailAddress] public string? Email { get; init; }

        [Required] [MinLength(6)] public string? Password { get; init; }

        [Required] public string? Fullname { get; init; }
    }
}