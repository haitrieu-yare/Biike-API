// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Application.Users.DTOs
{
    public class UserAddressDto
    {
        public int? UserAddressId { get; set; }
        public int? UserId { get; set; }
        public string? UserAddressName { get; set; }
        public string? UserAddressDetail { get; set; }
        public string? UserAddressCoordinate { get; set; }
        public string? UserAddressNote { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}