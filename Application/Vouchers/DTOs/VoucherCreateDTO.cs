using System;

namespace Application.Vouchers.DTOs
{
	public class VoucherCreateDTO
	{
		public int VoucherCategoryId { get; set; }
		public string VoucherName { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Quantity { get; set; }
		public int AmountOfPoint { get; set; }
		public string Description { get; set; } = string.Empty;
		public string TermsAndConditions { get; set; } = string.Empty;
	}
}