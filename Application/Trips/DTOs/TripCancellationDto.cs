using System;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Application.Trips.DTOs
{
    public class TripCancellationDto
    {
        [Required] public string? CancelReason { get; init; }
        public DateTime FinishedTime { get; } = CurrentTime.GetCurrentTime();
    }
}