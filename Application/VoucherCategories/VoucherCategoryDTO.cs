using System.ComponentModel.DataAnnotations;

namespace Application.VoucherCategories
{
	public class VoucherCategoryDTO
	{
		public int VoucherCategoryId { get; set; }

		[Required]
		public string CategoryName { get; set; }
	}
}