using System;

namespace Application.BikeAvailabilities.DTOs
{
    public class BikeAvailabilityDto
    {
        public int? BikeAvailabilityId { get; set; }
        public int? UserId { get; set; }
        public int? StationId { get; set; }
        public string? StationName { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}