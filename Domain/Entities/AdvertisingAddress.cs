// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class AdvertisingAddress
    {
        public int AdvertisingAddressId { get; set; }
        public int AdvertisingId { get; set; }
        public Advertising Advertising { get; set; } = null!;
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
    }
}