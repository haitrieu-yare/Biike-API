using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Stations;
using Application.Stations.DTOs;

namespace API.Controllers
{
	public class StationsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllStations(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListStations.Query(), ct));
		}

		[HttpGet("{stationId}")]
		public async Task<IActionResult> GetStationByStationId(int stationId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailStation.Query { StationId = stationId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateStation(StationCreateDTO stationCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateStation.Command { StationCreateDTO = stationCreateDTO }, ct));
		}

		[HttpPut("{stationId}")]
		public async Task<IActionResult> EditStation(int stationId, StationDTO newStationDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditStation.Command { StationId = stationId, NewStationDTO = newStationDTO }, ct));
		}

		[HttpDelete("{stationId}")]
		public async Task<IActionResult> DeleteStation(int stationId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteStation.Command { StationId = stationId }, ct));
		}
	}
}