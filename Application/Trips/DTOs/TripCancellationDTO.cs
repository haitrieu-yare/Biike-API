using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Trips.DTOs
{
	public class TripCancellationDTO
	{
		[Required]
		public string CancelReason { get; set; }
		public DateTime TimeFinished { get; set; }
	}
}