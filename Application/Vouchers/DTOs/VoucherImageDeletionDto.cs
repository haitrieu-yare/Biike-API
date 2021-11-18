using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherImageDeletionDto
    {
        [Required] public int? VoucherId { get; set; }
        [Required] public List<string>? VoucherImageIds { get; set; }
    }
}