using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Station
    {
        public int StationId { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Coordinate { get; set; } = string.Empty;
        public bool IsCentralPoint { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        
        public ICollection<BikeAvailability> BikeAvailabilities { get; set; } = new List<BikeAvailability>();

        [InverseProperty("Departure")] public ICollection<Route> DepartureRoutes { get; set; } = new List<Route>();

        [InverseProperty("Destination")] public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
    }
}