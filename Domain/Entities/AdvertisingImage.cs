// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Domain.Entities
{
    public class AdvertisingImage
    {
        public int AdvertisingImageId { get; set; }
        public int AdvertisingId { get; set; }
        public Advertising Advertising { get; set; } = null!;
        public string AdvertisingImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}