// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserAddressDto
    {
        public int? AddressId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressDetail { get; set; }
        public string? AddressCoordinate { get; set; }
        public string? Note { get; set; }
        public bool? IsDefault { get; set; }
    }
}