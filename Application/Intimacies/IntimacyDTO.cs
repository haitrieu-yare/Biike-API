using System;

namespace Application.Intimacies
{
	public class IntimacyDTO
	{
		public int UserOneId { get; set; }
		public int UserTwoId { get; set; }
		public bool IsBlock { get; set; }
		public DateTime BlockTime { get; set; }
		public DateTime? UnBlockTime { get; set; }
	}
}