using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace Application.Trips.DTOs
{
    public class TripCancellationDto
    {
        [Required] public string? CancelReason { get; init; }
    }
}