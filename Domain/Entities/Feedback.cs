using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
        public string? FeedbackContent { get; set; } 
        [Range(1, 5)] public int TripStar { get; set; }
        [Range(1, int.MaxValue)] public int TripTip { get; set; }
        public string? Criteria { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}