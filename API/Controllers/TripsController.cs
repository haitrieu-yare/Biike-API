using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Trips;
using Application.Trips.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripsController : BaseApiController
	{
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

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTrip(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateTrip(TripCreateDTO tripCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { TripCreateDTO = tripCreateDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditTripBiker(int id, TripBikerInfoDTO tripBikerInfoDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditTripBiker.Command { Id = id, TripBikerInfoDTO = tripBikerInfoDTO }, ct));
		}

		[HttpPut("{id}/progressTime")]
		public async Task<IActionResult> EditTripProgressTime(int id, string time, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
					new EditTripProcess.Command { Id = id, Time = DateTime.Parse(time) }, ct));
		}

		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> EditTripCancellation(int id, int userId,
			TripCancellationDTO tripCancellationDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditTripCancellation.Command
				{
					Id = id,
					UserId = userId,
					TripCancellationDTO = tripCancellationDTO
				}, ct));
		}
	}
}