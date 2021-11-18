// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Domain.Entities
{
    public class AdvertisementImage
    {
        public int AdvertisementImageId { get; set; }
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public string AdvertisementImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}