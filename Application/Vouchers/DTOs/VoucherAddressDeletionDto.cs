using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherAddressDeletionDto
    {
        [Required] public int? VoucherId { get; init; }
        [Required] public List<string>? VoucherAddressIds { get; init; }
    }
}