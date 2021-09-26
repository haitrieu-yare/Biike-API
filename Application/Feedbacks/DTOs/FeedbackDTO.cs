using System;

namespace Application.Feedbacks.DTOs
{
	public class FeedbackDTO
	{
		public int? UserId { get; set; }
		public int? TripId { get; set; }
		public string? FeedbackContent { get; set; }
		public int? TripStar { get; set; }
		public string? Criteria { get; set; }
		public DateTime? CreatedDate { get; set; }
	}
}