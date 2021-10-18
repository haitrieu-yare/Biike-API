using System.ComponentModel.DataAnnotations;

namespace Application.Location.DTOs
{
    public class LocationDto
    {
        [Required] public int? TripId { get; init; }
        [Required] public string? Coordinate { get; init; }
        [Required] public string? Time { get; init; }
    }
}