using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressCreationDto
    {
        [Required] public string? AddressName { get; set; }
        [Required] public string? AddressDetail { get; set; }
    }
}