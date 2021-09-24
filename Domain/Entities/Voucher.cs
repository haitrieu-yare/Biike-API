using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
	public class Voucher
	{
		public int VoucherId { get; set; }
		public int VoucherCategoryId { get; set; }
		public VoucherCategory VoucherCategory { get; set; }

		[Required]
		public string VoucherName { get; set; }

		[Required]
		public string Brand { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Quantity { get; set; }
		public int Remaining { get; set; }
		public int AmountOfPoint { get; set; }
		public string Description { get; set; }
		public string TermsAndConditions { get; set; }
		public ICollection<Redemption> Redemptions { get; set; }
	}
}