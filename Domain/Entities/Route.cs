using System;
using System.Collections.Generic;
// ReSharper disable CollectionNeverUpdated.Global

namespace Domain.Entities
{
    public class Route
    {
        public int RouteId { get; set; }
        public int DepartureId { get; set; }
        public Station Departure { get; set; } = null!;
        public int DestinationId { get; set; }
        public Station Destination { get; set; } = null!;
        public int AreaId { get; set; } = 1;
        public Area Area { get; set; } = null!;
        public int DefaultPoint { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public bool IsDeleted { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}