using System.ComponentModel.DataAnnotations;

namespace Domain
{
	public class Feedback
	{
		public int Id { get; set; }
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public int TripId { get; set; }
		public Trip Trip { get; set; }

		[Required]
		public string FeedbackContent { get; set; }
		public float Star { get; set; }

		[Required]
		public string Criteria { get; set; }
	}
}