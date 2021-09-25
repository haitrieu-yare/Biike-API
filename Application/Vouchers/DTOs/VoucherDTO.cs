using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
	public class VoucherDTO
	{
		public int VoucherId { get; set; }
		public int VoucherCategoryId { get; set; }

		[Required]
		public string VoucherName { get; set; }

		[Required]
		public string Brand { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Quantity { get; set; }
		public int Remaining { get; set; }
		public int AmountOfPoint { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string TermsAndConditions { get; set; }
	}
}