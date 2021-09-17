using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
	public class Area
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
		public bool IsDeleted { get; set; }
		public ICollection<Station> Stations { get; set; }
	}
}