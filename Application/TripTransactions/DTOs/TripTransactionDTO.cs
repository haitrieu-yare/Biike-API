using System;

namespace Application.TripTransactions.DTOs
{
	public class TripTransactionDTO
	{
		public int? TripTransactionId { get; set; }
		public int? TripId { get; set; }
		public int? WalletId { get; set; }
		public int? AmountPoint { get; set; }
		public DateTime? TransactionDate { get; set; }
	}
}