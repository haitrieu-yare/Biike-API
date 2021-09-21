using System.Threading;
using System.Threading.Tasks;
using Application.Trips;
using Application.Trips.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripsController : BaseApiController
	{
		[HttpGet("{id}/history")]
		public async Task<IActionResult> GetHistoryTrips(int id, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new HistoryList.Query { UserId = id, Role = role }, ct));
		}

		[HttpGet("{id}/upcoming")]
		public async Task<IActionResult> GetUpcomingTrips(int id, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new UpcomingList.Query { UserId = id, Role = role }, ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTrip(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateTrip(TripCreateDTO tripCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { TripCreateDto = tripCreateDto }, ct));
		}
	}
}