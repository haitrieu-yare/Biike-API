using System;

namespace Application.Routes.DTOs
{
	public class RouteDTO
	{
		public int? RouteId { get; set; }
		public int? DepartureId { get; set; }
		public int? DestinationId { get; set; }
		public int? DefaultPoint { get; set; }
		public DateTime? CreatedDate { get; set; }
		public bool? IsDeleted { get; set; }
	}
}