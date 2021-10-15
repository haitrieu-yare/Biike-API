using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class VoucherCategory
    {
        public int VoucherCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}