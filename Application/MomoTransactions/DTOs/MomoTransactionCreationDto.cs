using System.ComponentModel.DataAnnotations;

namespace Application.MomoTransactions.DTOs
{
    public class MomoTransactionCreationDto
    {
        [Required] public string? TransactionId { get; set; }
        [Required] public string? OrderId { get; set; }
        [Required] public int? Amount { get; set; }
    }
}