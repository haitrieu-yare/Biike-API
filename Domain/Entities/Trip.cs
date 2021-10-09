using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
	public class Trip
	{
		public int TripId { get; set; }
		public int KeerId { get; set; }
		public User Keer { get; set; } = null!;
		public int? BikerId { get; set; }
		public User? Biker { get; set; }
		public int RouteId { get; set; }
		public Route Route { get; set; } = null!;
		public DateTime BookTime { get; set; }
		public DateTime? PickupTime { get; set; }
		public DateTime? FinishedTime { get; set; }
		public int Status { get; set; } = (int) TripStatus.Finding;
		public string? PlateNumber { get; set; }
		public bool IsScheduled { get; set; } = true;
		public int? CancelPersonId { get; set; }
		public string? CancelReason { get; set; }
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
		public ICollection<Feedback> FeedbackList { get; set; } = new List<Feedback>();
		public ICollection<TripTransaction> TripTransactions { get; set; } = new List<TripTransaction>();
	}
}