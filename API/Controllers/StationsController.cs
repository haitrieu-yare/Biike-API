using System.Threading;
using System.Threading.Tasks;
using Application.Stations;
using Application.Stations.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class StationsController : BaseApiController
	{
		// Keer, Biker, Admin
		[HttpGet]
		public async Task<IActionResult> GetAllStations(int page, int limit, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new ListStations.Query { Page = page, Limit = limit, IsAdmin = validationDto.IsAdmin }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{stationId:int}")]
		public async Task<IActionResult> GetStationByStationId(int stationId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new DetailStation.Query { StationId = stationId, IsAdmin = validationDto.IsAdmin }, ct));
		}

		// Admin
		/// <summary>
		///     Yoooo
		/// </summary>
		/// <returns>
		///     201
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> CreateStation(StationCreateDto stationCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new CreateStation.Command { StationCreateDto = stationCreateDto }, ct));
		}

		// Admin
		[HttpPut("{stationId:int}")]
		public async Task<IActionResult> EditStation(int stationId, StationDto newStationDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new EditStation.Command { StationId = stationId, NewStationDto = newStationDto }, ct));
		}

		// Admin
		[HttpDelete("{stationId:int}")]
		public async Task<IActionResult> DeleteStation(int stationId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteStation.Command { StationId = stationId }, ct));
		}
	}
}