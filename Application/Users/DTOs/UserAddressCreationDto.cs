using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Application.Users.DTOs
{
    public class UserAddressCreationDto
    {
        [Required] public int? UserId { get; set; }
        [Required] public string? AddressName { get; set; }
        [Required] public string? AddressDetail { get; set; }
        [Required] public string? AddressCoordinate { get; set; }
        public string Note{ get; set; } = string.Empty;
    }
}