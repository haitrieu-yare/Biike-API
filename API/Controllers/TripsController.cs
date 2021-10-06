using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Trips;
using Application.Trips.DTOs;

namespace API.Controllers
{
	[Authorize]
	public class TripsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllTrips(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListTrips.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{userId}/history")]
		public async Task<IActionResult> GetHistoryTrips(
			int userId, int role, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new HistoryList.Query { Page = page, Limit = limit, UserId = userId, Role = role }, ct));
		}

		[HttpGet("{userId}/upcoming")]
		public async Task<IActionResult> GetUpcomingTrips(
			int userId, int role, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new UpcomingList.Query { Page = page, Limit = limit, UserId = userId, Role = role }, ct));
		}

		[HttpGet("historyPair")]
		public async Task<IActionResult> GetHistoryPairTrips(
			int userOneId, int userTwoId, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new HistoryPairList.Query
				{
					Page = page,
					Limit = limit,
					UserOneId = userOneId,
					UserTwoId = userTwoId
				}, ct));
		}

		[HttpGet("{tripId}")]
		public async Task<IActionResult> GetTrip(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailTrip.Query { TripId = tripId }, ct));
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