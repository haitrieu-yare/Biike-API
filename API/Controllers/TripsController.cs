using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Trips;
using Application.Trips.DTOs;

namespace API.Controllers
{
	public class TripsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllTrips(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListTrips.Query(), ct));
		}

		[HttpGet("{userId}/history")]
		public async Task<IActionResult> GetHistoryTrips(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new HistoryList.Query
			{ UserId = userId, Role = role }, ct));
		}

		[HttpGet("{userId}/upcoming")]
		public async Task<IActionResult> GetUpcomingTrips(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new UpcomingList.Query
			{ UserId = userId, Role = role }, ct));
		}

		[HttpGet("{tripId}")]
		public async Task<IActionResult> GetTrip(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailTrip.Query { TripId = tripId }, ct));
		}

		[HttpGet("historyPair")]
		public async Task<IActionResult> GetHistoryPairTrips(int userOneId, int userTwoId,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new HistoryPairList.Query
			{ UserOneId = userOneId, UserTwoId = userTwoId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateTrip(TripCreateDTO tripCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateTrip.Command { TripCreateDTO = tripCreateDto }, ct));
		}

		[HttpPut("{tripId}")]
		public async Task<IActionResult> EditTripBiker(int tripId, int bikerId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditTripBiker.Command { TripId = tripId, BikerId = bikerId }, ct));
		}

		[HttpPut("{tripId}/progressTime")]
		public async Task<IActionResult> EditTripProgressTime(int tripId, string time, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
					new EditTripProcess.Command { TripId = tripId, Time = DateTime.Parse(time) }, ct));
		}

		[HttpPut("{tripId}/cancel")]
		public async Task<IActionResult> EditTripCancellation(int tripId, int userId,
			TripCancellationDTO tripCancellationDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditTripCancellation.Command
				{
					TripId = tripId,
					UserId = userId,
					TripCancellationDTO = tripCancellationDTO
				}, ct));
		}
	}
}