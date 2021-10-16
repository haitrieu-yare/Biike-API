using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Wallets.DTOs
{
    public class WalletCreationDto
    {
        [Required] public int? UserId { get; set; }

        [Required] public DateTime? FromDate { get; set; }

        [Required] public DateTime? ToDate { get; set; }

        [Required] public int? Point { get; set; }

        [Required] public int? Status { get; set; }
    }
}