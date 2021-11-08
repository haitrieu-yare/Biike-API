// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Domain.Entities
{
    public class AdvertisingAddress
    {
        public int AdvertisingAddressId { get; set; }
        public int AdvertisingId { get; set; }
        public Advertising Advertising { get; set; } = null!;
        public string AdvertisingAddressName { get; set; } = string.Empty;
        public string AdvertisingAddressDetail { get; set; } = string.Empty;
        public string AdvertisingAddressCoordinate { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}