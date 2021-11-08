using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressDto
    {
        [Required] public int? VoucherAddressId { get; set; }
        [Required] public string? VoucherAddressName { get; set; }
        [Required] public string? VoucherAddressDetail { get; set; }
        [Required] public string? VoucherAddressCoordinate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}