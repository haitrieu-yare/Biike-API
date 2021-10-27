// ReSharper disable UnusedMember.Global

namespace Application.Users.DTOs
{
    public class UserAddressEditDto
    {
        public string? AddressName { get; set; }
        public string? AddressDetail { get; set; }
        public string? Note{ get; set; }
        public bool? IsDefault { get; set; }
    }
}