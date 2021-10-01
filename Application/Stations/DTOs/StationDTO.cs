using System;
using System.Text.Json.Serialization;

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

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public bool? IsDeleted { get; set; }
	}
}