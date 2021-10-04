using System.ComponentModel.DataAnnotations;

namespace Application.Feedbacks.DTOs
{
	public class FeedbackCreateDTO
	{
		[Required]
		public int? UserId { get; set; }

		[Required]
		public int? TripId { get; set; }

		[Required]
		public string? FeedbackContent { get; set; } = string.Empty;

		[Required]
		public int? TripStar { get; set; }

		[Required]
		public string? Criteria { get; set; } = string.Empty;
	}
}