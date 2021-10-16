using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserExistenceDto
    {
        [Required] public string? PhoneNumber { get; init; }

        [Required] public string? Email { get; init; }
    }
}