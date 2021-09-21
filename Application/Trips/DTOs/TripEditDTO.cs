using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Trips.DTOs
{
	public class TripBikerInfoDTO
	{
		public int BikerId { get; set; }
		public int Status { get; set; } = 1;

		[Required]
		public string NumberPlate { get; set; }
	}
}