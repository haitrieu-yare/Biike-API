using System.ComponentModel.DataAnnotations;

namespace Application.Bikes.DTOs
{
	public class BikeCreateDTO
	{
		[Required]
		public int? UserId { get; set; }

		[Required]
		public string? NumberPlate { get; set; } = string.Empty;

		[Required]
		public string? Color { get; set; } = string.Empty;

		[Required]
		public string? Brand { get; set; } = string.Empty;
	}
}