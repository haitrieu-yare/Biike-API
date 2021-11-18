using System;

namespace Application.Advertisements.DTOs
{
    public class AdvertisementDto
    {
        public int? AdvertisementId { get; set; }
        public int? CreatorId { get; set; }
        public string? AdvertisementUrl { get; set; } 
        public string? Brand { get; set; } 
        public string? Title { get; set; } 
        public bool? IsActive { get; set; }
        public int? TotalClickCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}