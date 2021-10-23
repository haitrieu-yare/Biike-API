using System;
using System.ComponentModel.DataAnnotations;
using Domain;

// ReSharper disable UnusedMember.Global

namespace Application.Trips.DTOs
{
    public class TripCancellationDto
    {
        [Required] public string? CancelReason { get; init; }
    }
}