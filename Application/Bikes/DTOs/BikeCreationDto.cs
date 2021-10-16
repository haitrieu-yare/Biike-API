using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Bikes.DTOs
{
    public class BikeCreationDto
    {
        [Required] public int? UserId { get; init; }

        [Required] public string? BikeOwner { get; init; }

        [Required] public string? Picture { get; init; }

        [Required] public string? Color { get; init; }

        [Required] public string? Brand { get; init; }
        [Required] [MinLength(4)] public string? PlateNumber { get; init; }
    }
}