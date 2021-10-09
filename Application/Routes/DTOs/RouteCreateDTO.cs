using System.ComponentModel.DataAnnotations;

namespace Application.Routes.DTOs
{
	public class RouteCreateDto
	{
		[Required]
		public int? DepartureId { get; set; }

		[Required]
		public int? DestinationId { get; set; }

		[Required]
		public int? DefaultPoint { get; set; }
	}
}