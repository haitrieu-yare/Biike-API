using System;

namespace Domain.Entities
{
    public class MomoTransaction
    {
        public int MomoTransactionId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string TransactionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int Point { get; set; }
        public double ConversionRate { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}