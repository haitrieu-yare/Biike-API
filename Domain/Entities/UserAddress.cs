// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class UserAddress
    {
        public int UserAddressId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
        public string Note { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}