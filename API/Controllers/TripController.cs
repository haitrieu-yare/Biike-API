using System.Threading;
using System.Threading.Tasks;
using Application.Trips;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetTrips(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTrip(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateTrip(TripDTO tripDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { TripDto = tripDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditTrip(int id, TripDTO tripDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command { Id = id, NewTripDto = tripDto }, ct));
		}
	}
}