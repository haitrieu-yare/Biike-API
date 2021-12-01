using System;

namespace Domain.Entities
{
    public class BikeAvailability
    {
        public int BikeAvailabilityId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}