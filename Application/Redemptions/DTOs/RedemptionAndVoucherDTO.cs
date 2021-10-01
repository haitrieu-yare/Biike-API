using System;

namespace Application.Redemptions.DTOs
{
	public class RedemptionAndVoucherDTO
	{
		public int? VoucherId { get; set; }
		public int? VoucherCategoryId { get; set; }
		public string? VoucherName { get; set; }
		public string? Brand { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? Description { get; set; }
		public string? TermsAndConditions { get; set; }
		public int? RedemptionId { get; set; }
		public int? WalletId { get; set; }
		public string? VoucherCode { get; set; }
		public int? VoucherPoint { get; set; }
		public bool? IsUsed { get; set; }
		public DateTime? RedemptionDate { get; set; }
	}
}