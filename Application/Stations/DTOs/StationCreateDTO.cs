using System;
using Domain;

namespace Application.Stations.DTOs
{
	public class StationCreateDTO
	{
		public int AreaId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Coordinate { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}