using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Stations;
using Application.Stations.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class StationsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllStations(CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(new ListStations.Query { IsAdmin = isAdmin }, ct));
		}

		[HttpGet("{stationId}")]
		public async Task<IActionResult> GetStationByStationId(int stationId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailStation.Query { IsAdmin = isAdmin, StationId = stationId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateStation(StationCreateDTO stationCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateStation.Command { StationCreateDTO = stationCreateDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{stationId}")]
		public async Task<IActionResult> EditStation(int stationId, StationDTO newStationDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditStation.Command { StationId = stationId, NewStationDTO = newStationDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{stationId}")]
		public async Task<IActionResult> DeleteStation(int stationId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteStation.Command { StationId = stationId }, ct));
		}
	}
}