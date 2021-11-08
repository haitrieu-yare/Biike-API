// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Application.Users.DTOs
{
    public class UserAddressDto
    {
        public int? UserAddressId { get; set; }
        public int? UserId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressDetail { get; set; }
        public string? AddressCoordinate { get; set; }
        public string? Note { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}