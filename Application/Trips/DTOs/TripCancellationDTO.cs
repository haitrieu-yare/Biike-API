using System;
using Domain;

namespace Application.Trips.DTOs
{
	public class TripCancellationDTO
	{
		public string CancelReason { get; set; } = string.Empty;
		public DateTime TimeFinished { get; set; } = CurrentTime.GetCurrentTime();
	}
}