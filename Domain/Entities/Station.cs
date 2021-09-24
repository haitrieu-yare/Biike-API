using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class Station
	{
		public int Id { get; set; }
		public int AreaId { get; set; }
		public Area Area { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public string Coordinate { get; set; }
		public bool IsDeleted { get; set; }

		[InverseProperty("Departure")]
		public ICollection<Route> DepartureRoutes { get; set; }

		[InverseProperty("Destination")]
		public ICollection<Route> DestinationRoutes { get; set; }
	}
}