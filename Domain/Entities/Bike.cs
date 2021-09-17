using Microsoft.EntityFrameworkCore;

namespace Domain
{
	[Index(nameof(PlateNumber), IsUnique = true)]
	public class Bike
	{
		public int Id { get; set; }
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }
		public string PlateNumber { get; set; }
		public string Color { get; set; }
		public string Brand { get; set; }
		public bool IsDeleted { get; set; }
	}
}