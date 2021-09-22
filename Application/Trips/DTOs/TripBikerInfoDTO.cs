using System.ComponentModel.DataAnnotations;

namespace Application.Trips.DTOs
{
	public class TripBikerInfoDTO
	{
		public int BikerId { get; set; }

		[Required]
		public string NumberPlate { get; set; }
	}
}