using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserLoginDto
    {
        [Required] [EmailAddress] public string? Email { get; init; }
        [Required] [MinLength(6)] public string? Password { get; init; }
    }
}