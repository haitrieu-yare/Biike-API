using System;
using System.Collections.Generic;

namespace Domain
{
	public class TripTransaction
	{
		public int Id { get; set; }
		public Trip Trip { get; set; }
		public Wallet Wallet { get; set; }
		public bool isBiker { get; set; }
		public int AmountOfPoint { get; set; }
		public DateTime TransactionDate { get; set; }
	}
}