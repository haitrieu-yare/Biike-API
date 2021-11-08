using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Application.Users.DTOs
{
    public class UserAddressCreationDto
    {
        [Required] public int? UserId { get; set; }
        [Required] public string? UserAddressName { get; set; }
        [Required] public string? UserAddressDetail { get; set; }
        [Required] public string? UserAddressCoordinate { get; set; }
        public string? UserAddressNote{ get; set; }
    }
}