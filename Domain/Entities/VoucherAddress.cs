// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Domain.Entities
{
    public class VoucherAddress
    {
        public int VoucherAddressId { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; } = null!;
        public int AddressId { get; set; }
        public Address Address { get; set; } = null!;
    }
}