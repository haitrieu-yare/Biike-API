using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherImageDto
    {
        [Required] public int? VoucherImageId { get; set; }
        [Required] public string? VoucherImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}