using System;
using System.Text.Json.Serialization;

namespace Application.Bikes.DTOs
{
    public class BikeDto
    {
        public int? BikeId { get; set; }
        public int? UserId { get; set; }
        public string? NumberPlate { get; set; }
        public string? BikeOwner { get; set; } = string.Empty;
        public string? Picture { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Brand { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedDate { get; set; }
    }
}