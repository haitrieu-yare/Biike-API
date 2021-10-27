using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Application.Addresses.DTOs
{
    public class AddressDto
    {
        [Required] public string? AddressName { get; set; } 
        [Required] public string? AddressDetail { get; set; }
    }
}