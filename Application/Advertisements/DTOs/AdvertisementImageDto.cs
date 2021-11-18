using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Advertisements.DTOs
{
    public class AdvertisementImageDto
    {
        [Required] public int? AdvertisementImageId { get; set; }
        [Required] public string? AdvertisementImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}