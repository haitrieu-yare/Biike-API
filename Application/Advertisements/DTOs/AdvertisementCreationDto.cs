using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Advertisements.DTOs
{
    public class AdvertisementCreationDto
    {
        [Required] public string? AdvertisementUrl { get; set; } 
        [Required] public string? Brand { get; set; } 
        [Required] public string? Title { get; set; } 
        [Required] public bool? IsActive { get; set; }
        [Required] public DateTime? StartDate { get; set; }
        [Required] public DateTime? EndDate { get; set; }
        [Required] public List<int>? AddressIds { get; set; }
        [Required] public List<string>? AdvertisementImages { get; set; }
    }
}