using System;

namespace Application.Wallets.DTOs
{
	public class WalletCreateDTO
	{
		public int AppUserId { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public int Point { get; set; }
		public int Status { get; set; }
	}
}