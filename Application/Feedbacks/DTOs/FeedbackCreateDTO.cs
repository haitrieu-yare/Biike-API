namespace Application.Feedbacks.DTOs
{
	public class FeedbackCreateDTO
	{
		public int UserId { get; set; }
		public int TripId { get; set; }
		public string FeedbackContent { get; set; }
		public double TripStar { get; set; }
		public string Criteria { get; set; }
	}
}