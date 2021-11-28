using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Trips.DTOs
{
    public class TripScheduleCreationDto
    {
        [Required] public int? KeerId { get; init; }
        [Required] public int? DepartureId { get; init; }
        [Required] public int? DestinationId { get; init; }
        [Required] public List<DateTime>? BookTime { get; init; }
    }
}