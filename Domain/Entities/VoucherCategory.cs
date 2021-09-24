using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
	public class VoucherCategory
	{
		public int VoucherCategoryId { get; set; }

		[Required]
		public string CategoryName { get; set; }
		public ICollection<Voucher> Vouchers { get; set; }
	}
}