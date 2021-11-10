using System;
using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Global

namespace Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public string AddressCoordinate { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public ICollection<VoucherAddress> VoucherAddresses { get; set; } = new List<VoucherAddress>();
        public ICollection<AdvertisingAddress> AdvertisingAddresses { get; set; } = new List<AdvertisingAddress>();
    }
}