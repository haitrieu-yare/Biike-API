// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class AdvertisingImage
    {
        public int AdvertisingImageId { get; set; }
        public int AdvertisingId { get; set; }
        public Advertising Advertising { get; set; } = null!;
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;
    }
}