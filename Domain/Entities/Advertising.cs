using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class Advertising
    {
        public int AdvertisingId { get; set; }
        public int CreatorId { get; set; }
        public User User { get; set; } = null!;
        public string AdvertisingUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();

        public ICollection<AdvertisingAddress> AdvertisingAddresses { get; set; } = new List<AdvertisingAddress>();
        public ICollection<AdvertisingImage> AdvertisingImages { get; set; } = new List<AdvertisingImage>();
    }
}