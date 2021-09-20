using System;

namespace Application.Trips.DTOs
{
	public class TripEditDTO
	{
		public int? BikerId { get; set; }
		public DateTime? PickupTime { get; set; }
		public DateTime? FinishedTime { get; set; }
		public int? Status { get; set; }
		public string PlateNumber { get; set; }
		public int? CancelPersonId { get; set; }
		public string CancelReason { get; set; }
	}
}