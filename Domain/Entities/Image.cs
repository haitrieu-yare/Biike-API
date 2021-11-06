using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        
        public AdvertisingImage? AdvertisingImage { get; set; }
        public VoucherImage? VoucherImage { get; set; } 
    }
}