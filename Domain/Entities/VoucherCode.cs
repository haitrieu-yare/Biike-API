using System;

namespace Domain.Entities
{
    public class VoucherCode
    {
        public int VoucherCodeId { get; set; }
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; } = null!;
        public string VoucherCodeName { get; set; } = string.Empty;
        public bool IsRedeemed { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}