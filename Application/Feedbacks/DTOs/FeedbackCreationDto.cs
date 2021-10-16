using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Feedbacks.DTOs
{
    public class FeedbackCreationDto
    {
        [Required] public int? UserId { get; init; }
        [Required] public int? TripId { get; init; }
        [Required] [Range(1, 5)] public int? TripStar { get; init; }

        [Required] public string? FeedbackContent { get; init; }

        [Required] public string? Criteria { get; init; }
    }
}