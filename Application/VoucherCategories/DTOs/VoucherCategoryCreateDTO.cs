using System.ComponentModel.DataAnnotations;

namespace Application.VoucherCategories.DTOs
{
	public class VoucherCategoryCreateDto
	{
		[Required]
		public string? CategoryName { get; set; }
	}
}