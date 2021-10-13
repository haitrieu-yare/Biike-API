using System.ComponentModel.DataAnnotations;

namespace Application.Feedbacks.DTOs
{
	public class FeedbackCreateDto
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required] public int? UserId { get; init; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required] public int? TripId { get; init; }

		[Required] public string? FeedbackContent { get; init; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required] [Range(1, 5)] public int? TripStar { get; init; }

		[Required] public string? Criteria { get; init; }
	}
}