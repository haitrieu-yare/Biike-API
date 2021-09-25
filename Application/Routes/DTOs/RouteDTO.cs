namespace Application.Routes.DTOs
{
	public class RouteDTO
	{
		public int? Id { get; set; }
		public int? DepartureId { get; set; }
		public int? DestinationId { get; set; }
		public int? DefaultPoint { get; set; }
		public bool? IsDeleted { get; set; }
	}
}