using System;

namespace Application.Trips.DTOs
{
	public class TripDetailDTO
	{
		public int? Id { get; set; }
		public int? KeerId { get; set; }
		public int? BikerId { get; set; }
		public int? RouteId { get; set; }
		public DateTime BookTime { get; set; }
		public DateTime? PickupTime { get; set; }
		public DateTime? FinishedTime { get; set; }
		public int? Status { get; set; }
		public string? PlateNumber { get; set; }
		public bool? IsScheduled { get; set; }
		public int? CancelPersonId { get; set; }
		public string? CancelReason { get; set; }
		public DateTime? CreatedDate { get; set; }
	}
}