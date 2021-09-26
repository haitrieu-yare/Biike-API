using System;
using Domain;

namespace Application.Feedbacks.DTOs
{
	public class FeedbackCreateDTO
	{
		public int UserId { get; set; }
		public int TripId { get; set; }
		public string FeedbackContent { get; set; } = string.Empty;
		public int TripStar { get; set; }
		public string Criteria { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}