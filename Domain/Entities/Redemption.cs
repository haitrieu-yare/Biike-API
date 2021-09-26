using System;

namespace Domain.Entities
{
	public class Redemption
	{
		public int RedemptionId { get; set; }
		public int WalletId { get; set; }
		public Wallet Wallet { get; set; } = null!;
		public int VoucherId { get; set; }
		public Voucher Voucher { get; set; } = null!;
		public string VoucherCode { get; set; } = String.Empty;
		public int VoucherPoint { get; set; }
		public bool IsUsed { get; set; } = false;
		public DateTime RedemptionDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}