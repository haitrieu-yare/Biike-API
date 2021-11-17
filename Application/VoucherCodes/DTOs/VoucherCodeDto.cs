using System;

namespace Application.VoucherCodes.DTOs
{
    public class VoucherCodeDto
    {
        public int? VoucherCodeId { get; set; }
        public int? VoucherId { get; set; }
        public string? VoucherCodeName { get; set; } 
        public bool? IsRedeemed { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}