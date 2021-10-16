using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Routes.DTOs
{
    public class RouteCreationDto
    {
        [Required] public int? DepartureId { get; init; }

        [Required] public int? DestinationId { get; init; }

        [Required] public int? DefaultPoint { get; init; }
    }
}