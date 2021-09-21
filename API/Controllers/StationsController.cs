using System.Threading;
using System.Threading.Tasks;
using Application.Stations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class StationsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetStations(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetStation(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateStation(StationDTO stationDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { StationDTO = stationDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditStation(int id, StationDTO newStationDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command { Id = id, NewStationDTO = newStationDto }, ct));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteStation(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Delete.Command { Id = id }, ct));
		}
	}
}