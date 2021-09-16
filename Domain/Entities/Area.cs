using System.Collections.Generic;

namespace Domain
{
	public class Area
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsDeleted { get; set; }

		public ICollection<Station> Stations { get; set; }
	}
}