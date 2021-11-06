using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
    public class VoucherImageDto
    {
        [Required] public int? ImageId { get; set; }
        [Required] public string? ImageUrl { get; set; }
    }
}