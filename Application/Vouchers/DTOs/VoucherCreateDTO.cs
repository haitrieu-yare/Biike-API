using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Vouchers.DTOs
{
	public class VoucherCreateDto
	{
		[Required] public int? VoucherCategoryId { get; set; }

		[Required] public string? VoucherName { get; set; } = string.Empty;

		[Required] public string? Brand { get; set; } = string.Empty;

		[Required] public DateTime? StartDate { get; set; }

		[Required] public DateTime? EndDate { get; set; }

		[Required] public int? Quantity { get; set; }

		[Required] public int? AmountOfPoint { get; set; }

		[Required] public string? Description { get; set; } = string.Empty;

		[Required] public string? TermsAndConditions { get; set; } = string.Empty;
	}
}