using System;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Application.Trips.DTOs
{
	public class TripCreateDto
	{
		[Required] public int? KeerId { get; set; }
		[Required] public int? RouteId { get; set; }
		[Required] public DateTime? BookTime { get; set; } = CurrentTime.GetCurrentTime();
		[Required] public bool? IsScheduled { get; set; }
	}
}