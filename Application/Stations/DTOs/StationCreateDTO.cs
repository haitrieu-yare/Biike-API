using System.ComponentModel.DataAnnotations;

namespace Application.Stations.DTOs
{
    public class StationCreateDto
    {
        [Required] public int? AreaId { get; set; }

        [Required] public string? Name { get; set; }

        [Required] public string? Address { get; set; }

        [Required] public string? Coordinate { get; set; }
    }
}