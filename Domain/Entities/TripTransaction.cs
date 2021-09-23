using System;

namespace Domain
{
	public class TripTransaction
	{
		public int Id { get; set; }
		public int TripId { get; set; }
		public Trip Trip { get; set; }
		public int WalletId { get; set; }
		public Wallet Wallet { get; set; }
		public int AmountOfPoint { get; set; }
		public DateTime TransactionDate { get; set; }
	}
}