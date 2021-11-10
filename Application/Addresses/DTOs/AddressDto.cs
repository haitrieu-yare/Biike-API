using System;

// ReSharper disable UnusedMember.Global

namespace Application.Addresses.DTOs
{
    public class AddressDto
    {
        public int? AddressId { get; set; }
        public string? AddressName { get; set; } 
        public string? AddressDetail { get; set; }
        public string? AddressCoordinate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}