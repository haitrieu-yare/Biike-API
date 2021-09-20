using System;

namespace Application.Trips.DTOs
{
	public class KeerHistoryTripDTO
	{
		public int Id { get; set; }
		public int? BikerId { get; set; }
		public string Avatar { get; set; }
		public string UserFullname { get; set; }
		public DateTime BookTime { get; set; }
		public int Status { get; set; }
		public string DepartureName { get; set; }
		public string DestinationName { get; set; }
	}
}