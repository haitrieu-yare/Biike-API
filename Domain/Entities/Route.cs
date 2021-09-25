using System;
using System.Collections.Generic;

namespace Domain.Entities
{
	public class Route
	{
		public int RouteId { get; set; }
		public int DepartureId { get; set; }
		public Station Departure { get; set; }
		public int DestinationId { get; set; }
		public Station Destination { get; set; }
		public int DefaultPoint { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public bool IsDeleted { get; set; } = false;
		public ICollection<Trip> Trips { get; set; }
	}
}