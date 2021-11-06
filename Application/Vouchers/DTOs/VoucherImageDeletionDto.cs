using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherImageDeletionDto
    {
        [Required] public int? VoucherId { get; init; }
        [Required] public List<string>? ImageIds { get; init; }
    }
}