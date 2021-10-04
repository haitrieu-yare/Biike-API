using System;
using System.Text.Json.Serialization;

namespace Application.Feedbacks.DTOs
{
	public class FeedbackDTO
	{
		public int? FeedbackId { get; set; }
		public int? UserId { get; set; }
		public int? TripId { get; set; }
		public string? FeedbackContent { get; set; }
		public int? TripStar { get; set; }
		public string? Criteria { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? CreatedDate { get; set; }
	}
}