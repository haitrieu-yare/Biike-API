using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
	public class Redemption
	{
		public int RedemptionId { get; set; }
		public int WalletId { get; set; }
		public int VoucherId { get; set; }
		public Voucher Voucher { get; set; }

		[Required]
		public string VourcherCode { get; set; }
		public bool IsUsed { get; set; }
		public DateTime RedemptionDate { get; set; }
	}
}