using System.ComponentModel.DataAnnotations;

namespace Application.Redemptions.DTOs
{
	public class RedemptionCreateDto
	{
		[Required]
		public int? UserId { get; set; }

		[Required]
		public int? VoucherId { get; set; }
	}
}