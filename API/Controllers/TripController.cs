using System.Threading;
using System.Threading.Tasks;
using Application.Trips;
using Application.Trips.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripController : BaseApiController
	{
		[HttpGet("history/user={userId}&role={role}")]
		public async Task<IActionResult> GetHistoryTrips(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new HistoryList.Query { UserId = userId, Role = role }, ct));
		}

		[HttpGet("upcoming/user={userId}&role={role}")]
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
		public async Task<IActionResult> CreateTrip(TripCreateDTO tripCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { TripCreateDto = tripCreateDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditTrip(int id, TripEditDTO tripEditDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command { Id = id, TripEditDto = tripEditDto }, ct));
		}
	}
}