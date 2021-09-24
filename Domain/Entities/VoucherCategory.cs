using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
	[Index(nameof(CategoryName), IsUnique = true)]
	public class VoucherCategory
	{
		public int VoucherCategoryId { get; set; }

		[Required]
		public string CategoryName { get; set; }
		public ICollection<Voucher> Vouchers { get; set; }
	}
}