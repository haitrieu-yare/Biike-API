using System.ComponentModel.DataAnnotations;

namespace Application.Bikes.DTOs
{
	public class BikeCreateDto
	{
		[Required] public int? UserId { get; set; }
		[Required] [MinLength(4)] public string? NumberPlate { get; set; }

		[Required] public string? BikeOwner { get; set; }

		[Required] public string? Picture { get; set; }

		[Required] public string? Color { get; set; }

		[Required] public string? Brand { get; set; }
	}
}