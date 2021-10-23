using System;
using System.Text.Json.Serialization;

namespace Application.Routes.DTOs
{
    public class RouteDto
    {
        public int? RouteId { get; set; }
        public int? DepartureId { get; set; }
        public int? DestinationId { get; set; }
        public int? AreaId { get; set; }
        public string? DepartureName { get; set; }
        public string? DestinationName { get; set; }
        
        public int? DefaultPoint { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsDeleted { get; set; }
    }
}