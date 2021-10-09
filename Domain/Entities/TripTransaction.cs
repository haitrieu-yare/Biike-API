using System;

namespace Domain.Entities
{
	public class TripTransaction
	{
		public int TripTransactionId { get; set; }
		public int TripId { get; set; }
		public Trip Trip { get; set; } = null!;
		public int WalletId { get; set; }
		public Wallet Wallet { get; set; } = null!;
		public int AmountOfPoint { get; set; }
		public DateTime TransactionDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}