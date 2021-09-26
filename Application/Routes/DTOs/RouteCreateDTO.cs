using System;
using Domain;

namespace Application.Routes.DTOs
{
	public class RouteCreateDTO
	{
		public int DepartureId { get; set; }
		public int DestinationId { get; set; }
		public int DefaultPoint { get; set; }
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}