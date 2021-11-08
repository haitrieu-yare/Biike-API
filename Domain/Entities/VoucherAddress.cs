// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;

namespace Domain.Entities
{
    public class VoucherAddress
    {
        public int VoucherAddressId { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; } = null!;
        public string VoucherAddressName { get; set; } = string.Empty;
        public string VoucherAddressDetail { get; set; } = string.Empty;
        public string VoucherAddressCoordinate { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}