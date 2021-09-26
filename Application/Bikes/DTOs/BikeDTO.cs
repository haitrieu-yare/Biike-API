using System;

namespace Application.Bikes.DTOs
{
	public class BikeDTO
	{
		public int? BikeId { get; set; }
		public int? UserId { get; set; }
		public string? NumberPlate { get; set; }
		public string? Color { get; set; }
		public string? Brand { get; set; }
		public DateTime? CreatedDate { get; set; }
	}
}