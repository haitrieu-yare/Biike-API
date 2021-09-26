using System;
using Domain;

namespace Application.Intimacies.DTOs
{
	public class IntimacyCreateDTO
	{
		public int UserOneId { get; set; }
		public int UserTwoId { get; set; }
		public bool IsBlock { get; set; } = true;
		public DateTime BlockTime { get; set; } = CurrentTime.GetCurrentTime();
	}
}