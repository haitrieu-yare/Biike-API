using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Trips.DTOs
{
    public class TripCreationDto
    {
        [Required] public int? KeerId { get; init; }
        [Required] public int? DepartureId { get; init; }
        [Required] public int? DestinationId { get; init; }
        [Required] public DateTime? BookTime { get; init; }
        [Required] public bool? IsScheduled { get; init; }
    }
}