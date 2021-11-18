using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Advertisements.DTOs
{
    public class AdvertisementImageDeletionDto
    {
        [Required] public int? AdvertisementId { get; set; }
        [Required] public List<string>? AdvertisementImageIds { get; set; }
    }
}