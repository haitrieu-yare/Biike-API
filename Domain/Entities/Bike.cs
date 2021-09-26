using System;

namespace Domain.Entities
{
	public class Bike
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; } = null!;
		public string PlateNumber { get; set; } = string.Empty;
		public string Color { get; set; } = string.Empty;
		public string Brand { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
	}
}