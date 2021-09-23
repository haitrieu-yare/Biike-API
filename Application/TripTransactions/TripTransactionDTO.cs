using System;

namespace Application.TripTransactions
{
	public class TripTransactionDTO
	{
		public int TransactionId { get; set; }
		public int TripId { get; set; }
		public int WalletId { get; set; }
		public int AmountPoint { get; set; }
		public DateTime TransactionDate { get; set; }
	}
}