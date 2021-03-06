using System;

namespace Application.TripTransactions.DTOs
{
    public class TripTransactionDto
    {
        public int? TripTransactionId { get; set; }
        public int? TripId { get; set; }
        public int? WalletId { get; set; }
        public int? AmountOfPoint { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}