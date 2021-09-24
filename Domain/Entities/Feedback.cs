using System.ComponentModel.DataAnnotations;

namespace Domain
{
	public class Feedback
	{
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public int TripId { get; set; }
		public Trip Trip { get; set; }

		[Required]
		public string FeedbackContent { get; set; }
		public int Star { get; set; }

		[Required]
		public string Criteria { get; set; }
	}
}