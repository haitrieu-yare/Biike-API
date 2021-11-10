using System.ComponentModel.DataAnnotations;

namespace Application.Addresses.DTOs
{
    public class AddressCreationDto
    {
        [Required] public string? AddressName { get; set; } 
        [Required] public string? AddressDetail { get; set; }
        [Required] public string? AddressCoordinate { get; set; }
    }
}