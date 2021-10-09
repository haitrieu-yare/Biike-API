using System;

namespace Domain.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        public string FeedbackContent { get; set; } = string.Empty;
        public int Star { get; set; }
        public string Criteria { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}