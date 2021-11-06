// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class VoucherImage
    {
        public int VoucherImageId { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; } = null!;
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;
    }
}