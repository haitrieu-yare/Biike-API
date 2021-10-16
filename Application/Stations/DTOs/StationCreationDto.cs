using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Stations.DTOs
{
    public class StationCreationDto
    {
        [Required] public int? AreaId { get; init; }

        [Required] public string? Name { get; init; }

        [Required] public string? Address { get; init; }

        [Required] public string? Coordinate { get; init; }
    }
}