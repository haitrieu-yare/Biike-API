using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
	public class Wallet
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; } = null!;
		public DateTime FromDate { get; set; } = CurrentTime.GetCurrentTime();
		public DateTime ToDate { get; set; } = CurrentTime.GetCurrentTime();
		public int Point { get; set; }
		public int Status { get; set; } = (int)WalletStatus.Current;
		public ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
		public ICollection<TripTransaction> TripTransactions { get; set; } = new List<TripTransaction>();
	}
}