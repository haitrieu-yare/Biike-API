// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class AdvertisementAddress
    {
        public int AdvertisementAddressId { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
    }
}