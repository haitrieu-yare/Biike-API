using System;

namespace Domain
{
	public class Intimacy
	{
		public int UserOneId { get; set; }
		public AppUser UserOne { get; set; }
		public int UserTwoId { get; set; }
		public AppUser UserTwo { get; set; }
		public bool IsBlock { get; set; }
		public DateTime BlockTime { get; set; }
		public DateTime? UnblockTime { get; set; }
	}
}