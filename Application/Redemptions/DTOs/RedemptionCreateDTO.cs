using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Redemptions.DTOs
{
	public class RedemptionCreateDTO
	{
		[Required]
		public int? WalletId { get; set; }

		[Required]
		public int? VoucherId { get; set; }

		[Required]
		public DateTime? RedemptionDate { get; set; }
	}
}