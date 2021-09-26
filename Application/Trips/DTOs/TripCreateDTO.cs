using System;
using Domain;

namespace Application.Trips.DTOs
{
	public class TripCreateDTO
	{
		public int KeerId { get; set; }
		public int RouteId { get; set; }
		public DateTime BookTime { get; set; } = CurrentTime.GetCurrentTime();
		public bool IsScheduled { get; set; } = false;
	}
}