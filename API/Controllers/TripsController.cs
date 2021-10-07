using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Trips;
using Application.Trips.DTOs;
using Domain.Enums;

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

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpGet("{userId}/history")]
		public async Task<IActionResult> GetHistoryTrips(
			int userId, int role, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new HistoryList.Query { Page = page, Limit = limit, UserId = userId, Role = role }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpGet("{userId}/upcoming")]
		public async Task<IActionResult> GetUpcomingTrips(
			int userId, int role, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new UpcomingList.Query { Page = page, Limit = limit, UserId = userId, Role = role }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
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
		public async Task<IActionResult> GetTripDetail(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailTrip.Query { TripId = tripId }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpGet("{tripId}/details")]
		public async Task<IActionResult> GetTripDetailInfo(int tripId, CancellationToken ct)
		{
			int role = 0;
			foreach (RoleStatus roleStatus in Enum.GetValues(typeof(RoleStatus)))
			{
				if (HttpContext.User.IsInRole(((int)roleStatus).ToString()))
				{
					role = (int)roleStatus;
				}
			}

			var userRequestIdClaim = HttpContext.User.FindFirst(c => c.Type.Equals("user_id"));
			string? userRequestId = userRequestIdClaim?.Value;

			if (string.IsNullOrEmpty(userRequestId))
			{
				return BadRequest("Can't get userId who send the request.");
			}

			return HandleResult(await Mediator.Send(
				new DetailTripInfo.Query
				{
					TripId = tripId,
					Role = role,
					UserRequestId = int.Parse(userRequestId),
				}, ct));
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