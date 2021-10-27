using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        
        public ICollection<AdvertisingAddress> AdvertisingAddresses { get; set; } = new List<AdvertisingAddress>();
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    }
}