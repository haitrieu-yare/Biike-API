using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Users.DTOs
{
    public class UserAddressDto
    {
        [Required] public string? AddressName { get; set; }
        [Required] public string? AddressDetail { get; set; }
        public string Note{ get; set; } = string.Empty;
    }
}