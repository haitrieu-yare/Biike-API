using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressDto
    {
        [Required] public int? AddressId { get; set; }
        public string? AddressName { get; set; }
        public string? AddressDetail { get; set; }
        public string? AddressCoordinate { get; set; }
    }
}