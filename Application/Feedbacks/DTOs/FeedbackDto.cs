using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Feedbacks.DTOs
{
    public class FeedbackDto
    {
        public int? FeedbackId { get; set; }
        public int? UserId { get; set; }
        public string? UserFullname { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TripId { get; set; }

        [Range(1, 5)] public int? TripStar { get; set; }
        public string? FeedbackContent { get; set; }

        public string? Criteria { get; set; }
        public int? TripTip { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedDate { get; set; }
    }
}