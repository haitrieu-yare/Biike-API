using System;

namespace Application.Redemptions.DTOs
{
	public class RedemptionDTO
	{
		public int? RedemptionId { get; set; }
		public int? WalletId { get; set; }
		public int? VoucherId { get; set; }
		public string? VoucherCode { get; set; }
		public int? VoucherPoint { get; set; }
		public bool? IsUsed { get; set; }
		public DateTime? RedemptionDate { get; set; }
	}
}