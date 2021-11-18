using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace Domain.Entities
{
    public class Advertisement
    {
        public int AdvertisementId { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;
        public string AdvertisementUrl { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TotalClickCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();

        public ICollection<AdvertisementAddress> AdvertisementAddresses { get; set; } = new List<AdvertisementAddress>();
        public ICollection<AdvertisementImage> AdvertisementImages { get; set; } = new List<AdvertisementImage>();
    }
}