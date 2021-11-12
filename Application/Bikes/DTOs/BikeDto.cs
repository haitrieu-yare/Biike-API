using System;
using System.Text.Json.Serialization;

namespace Application.Bikes.DTOs
{
    public class BikeDto
    {
        public int? BikeId { get; set; }
        public int? UserId { get; set; }
        public string? PlateNumber { get; set; }
        public string? BikeOwner { get; set; }
        public string? BikePicture { get; set; }
        public string? BikeLicensePicture { get; set; }
        public string? PlateNumberPicture { get; set; }
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public string? BikeType { get; set; }
        public string? BikeVolume { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedDate { get; set; }
    }
}