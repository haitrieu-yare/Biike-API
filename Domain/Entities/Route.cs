using System.Collections.Generic;

namespace Domain
{
	public class Route
	{
		public int Id { get; set; }
		public Station Departure { get; set; }
		public Station Destination { get; set; }
		public int DefaultPoint { get; set; }
		public bool IsDeleted { get; set; }

		public ICollection<Trip> Trips { get; set; }
	}
}