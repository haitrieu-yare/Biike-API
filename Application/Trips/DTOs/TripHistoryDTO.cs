using System;

namespace Application.Trips.DTOs
{
	public class TripHistoryDTO
	{
		public int TripId { get; set; }
		public int? UserId { get; set; }
		public string Avatar { get; set; }
		public string UserFullname { get; set; }
		public DateTime TimeBook { get; set; }
		public int TripStatus { get; set; }
		public string StartingPointName { get; set; }
		public string DestinationName { get; set; }
	}
}