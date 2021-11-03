using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Feedbacks.DTOs
{
    public class FeedbackCreationDto
    {
        [Required] public int? UserId { get; init; }
        [Required] public int? TripId { get; init; }
        [Required] [Range(1, 5)] public int? TripStar { get; init; }
        [Range(1, int.MaxValue)]  public int? TripTip { get; init; }

        public string? FeedbackContent { get; init; }

        public string? Criteria { get; init; }
    }
}