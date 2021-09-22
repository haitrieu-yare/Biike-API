using System;

namespace Application.Trips.DTOs
{
	public class TripCreateDTO
	{
		public int KeerId { get; set; }
		public int RouteId { get; set; }
		public DateTime BookTime { get; set; }
		public bool IsScheduled { get; set; }
	}
}