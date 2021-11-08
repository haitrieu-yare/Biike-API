using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressCreationDto
    {
        [Required] public string? VoucherAddressName { get; set; }
        [Required] public string? VoucherAddressDetail { get; set; }
        [Required] public string? VoucherAddressCoordinate { get; set; }
    }
}