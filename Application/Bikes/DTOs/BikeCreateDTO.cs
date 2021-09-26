using System;
using Domain;

namespace Application.Bikes.DTOs
{
	public class BikeCreateDTO
	{
		public int UserId { get; set; }
		public string NumberPlate { get; set; } = string.Empty;
		public string Color { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}