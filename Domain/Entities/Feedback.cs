using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
	public class Feedback
	{
		public int Id { get; set; }
		public AppUser AppUser { get; set; }
		public Trip Trip { get; set; }
		public string FeedbackContent { get; set; }
		public double Star { get; set; }
		public string Criteria { get; set; }
	}
}