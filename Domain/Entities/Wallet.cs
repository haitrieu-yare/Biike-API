using System;
using System.Collections.Generic;

namespace Domain
{
	public class Wallet
	{
		public int Id { get; set; }
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public int Point { get; set; }
		public int Status { get; set; }
		public ICollection<TripTransaction> TripTransactions { get; set; }
	}
}