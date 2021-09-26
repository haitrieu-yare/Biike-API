using System;

namespace Application.Stations.DTOs
{
	public class StationDTO
	{
		public int? StationId { get; set; }
		public int? AreaId { get; set; }
		public string? Name { get; set; }
		public string? Address { get; set; }
		public string? Coordinate { get; set; }
		public DateTime? CreatedDate { get; set; }
		public bool? IsDeleted { get; set; }
	}
}