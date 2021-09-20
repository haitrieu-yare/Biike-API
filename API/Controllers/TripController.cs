using System.Threading;
using System.Threading.Tasks;
using Application.Trips;
using Application.Trips.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripController : BaseApiController
	{
		[HttpGet("history/{userId}/{role}")]
		public async Task<IActionResult> GetHistoryTrips(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new HistoryList.Query { UserId = userId, Role = role }, ct));
		}

		[HttpGet("upcoming/{userId}/{role}")]
		public async Task<IActionResult> GetUpcomingTrips(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new UpcomingList.Query { UserId = userId, Role = role }, ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTrip(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateTrip(TripHistoryDTO tripDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { TripDto = tripDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditTrip(int id, TripHistoryDTO tripDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command { Id = id, NewTripDto = tripDto }, ct));
		}
	}
}