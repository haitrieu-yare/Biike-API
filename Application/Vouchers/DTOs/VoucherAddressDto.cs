using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressDto
    {
        [Required] public int? VoucherAddressId { get; set; }
        [Required] public string? AddressName { get; set; }
        [Required] public string? AddressDetail { get; set; }
        [Required] public string? AddressCoordinate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}