using System.ComponentModel.DataAnnotations;

namespace Application.Intimacies.DTOs
{
	public class IntimacyCreateEditDto
	{
		[Required]
		public int? UserOneId { get; set; }

		[Required]
		public int? UserTwoId { get; set; }
	}
}