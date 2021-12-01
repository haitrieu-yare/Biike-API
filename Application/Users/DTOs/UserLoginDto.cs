using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^.*@((fpt\.edu\.vn)|(fe\.edu\.vn))$", ErrorMessage = "Must use fpt email.")]
        public string? Email { get; init; }

        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public string? Password { get; init; }

        [Required] public bool? IsAdmin { get; set; }
    }
}