using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Wallets.DTOs
{
	public class WalletCreateDTO
	{
		[Required]
		public int? AppUserId { get; set; }

		[Required]
		public DateTime? FromDate { get; set; }

		[Required]
		public DateTime? ToDate { get; set; }

		[Required]
		public int? Point { get; set; }

		[Required]
		public int? Status { get; set; }
	}
}