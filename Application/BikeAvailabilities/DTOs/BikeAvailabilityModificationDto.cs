using System;
using System.ComponentModel.DataAnnotations;

namespace Application.BikeAvailabilities.DTOs
{
    public class BikeAvailabilityModificationDto
    {
        [Required] public int? StationId { get; set; }
        [Required] public DateTime? FromTime { get; set; }
        [Required] public DateTime? ToTime { get; set; }
    }
}