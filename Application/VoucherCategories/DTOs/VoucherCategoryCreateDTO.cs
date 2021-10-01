using System.ComponentModel.DataAnnotations;

namespace Application.VoucherCategories.DTOs
{
	public class VoucherCategoryCreateDTO
	{
		[Required]
		public string? CategoryName { get; set; }
	}
}