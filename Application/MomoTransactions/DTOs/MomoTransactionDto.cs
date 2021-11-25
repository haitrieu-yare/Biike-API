using System;

namespace Application.MomoTransactions.DTOs
{
    public class MomoTransactionDto
    {
        public int? MomoTransactionId { get; set; }
        public int? UserId { get; set; }
        public string? TransactionId { get; set; } 
        public string? OrderId { get; set; }
        public int? Amount { get; set; }
        public int? Point { get; set; }
        public double? ConversionRate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}