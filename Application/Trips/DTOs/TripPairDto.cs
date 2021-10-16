using System;

namespace Application.Trips.DTOs
{
    public class TripPairDto
    {
        public int? TripId { get; set; }
        public int? UserId { get; set; }
        public string? Avatar { get; set; }
        public string? UserFullname { get; set; }
        public DateTime? BookTime{ get; set; }
        public int? TripStatus { get; set; }
        public string? DepartureName { get; set; }
        public string? DestinationName { get; set; }
    }
}