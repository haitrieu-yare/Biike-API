using System;

namespace Application.Wallets.DTOs
{
	public class WalletDTO
	{
		public int? WalletId { get; set; }
		public int? UserId { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public int? Point { get; set; }
		public int? Status { get; set; }
	}
}