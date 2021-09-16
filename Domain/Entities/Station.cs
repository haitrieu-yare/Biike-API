using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
	public class Station
	{
		public int Id { get; set; }

		[InverseProperty("Departure")]
		public ICollection<Route> DepartureRoutes { get; set; }

		[InverseProperty("Destination")]
		public ICollection<Route> DestinationRoutes { get; set; }

		public string Name { get; set; }
		public string Address { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public bool IsDeleted { get; set; }

		public Area Area { get; set; }
	}
}