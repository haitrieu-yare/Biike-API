using System;
using System.Collections.Generic;

namespace Domain.Entities
{
	public class Trip
	{
		public int Id { get; set; }
		public int KeerId { get; set; }
		public AppUser Keer { get; set; }
		public int? BikerId { get; set; }
		public AppUser Biker { get; set; }
		public int RouteId { get; set; }
		public Route Route { get; set; }
		public DateTime BookTime { get; set; }
		public DateTime? PickupTime { get; set; }
		public DateTime? FinishedTime { get; set; }
		public int Status { get; set; }
		public string PlateNumber { get; set; }
		public bool IsScheduled { get; set; }
		public int? CancelPersonId { get; set; }
		public string CancelReason { get; set; }
		public ICollection<Feedback> FeedbackList { get; set; }
		public ICollection<TripTransaction> TripTransactions { get; set; }
	}
}