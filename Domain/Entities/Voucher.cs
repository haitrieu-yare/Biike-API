using System;
using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Domain.Entities
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public int VoucherCategoryId { get; set; }
        public VoucherCategory VoucherCategory { get; set; } = null!;
        public string VoucherName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = CurrentTime.GetCurrentTime();
        public DateTime EndDate { get; set; } = CurrentTime.GetCurrentTime();
        public int Quantity { get; set; }
        public int Remaining { get; set; }
        public int AmountOfPoint { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TermsAndConditions { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public ICollection<VoucherAddress> VoucherAddresses { get; set; } = new List<VoucherAddress>();
        public ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
    }
}