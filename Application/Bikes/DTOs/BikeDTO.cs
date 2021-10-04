using System;
using System.Text.Json.Serialization;

namespace Application.Bikes.DTOs
{
	public class BikeDTO
	{
		public int? BikeId { get; set; }
		public int? UserId { get; set; }
		public string? NumberPlate { get; set; }
		public string? Color { get; set; }
		public string? Brand { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? CreatedDate { get; set; }
	}
}