using System;
using System.Collections.Generic;

namespace Application.Advertisements.DTOs
{
    public class AdvertisementEditDto
    {
        public string? AdvertisementUrl { get; set; } 
        public string? Brand { get; set; } 
        public string? Title { get; set; } 
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int>? AddressIds { get; set; }
        public List<AdvertisementImageDto>? AdvertisementImages { get; set; }
    }
}