using System.Threading;
using System.Threading.Tasks;
using Application.Bikes;
using Application.Bikes.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class BikesController : BaseApiController
	{
		[HttpGet("{userId}")]
		public async Task<IActionResult> GetBike(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailBike.Query { UserId = userId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateBike(BikeCreateDTO bikeCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateBike.Command { BikeCreateDTO = bikeCreateDTO }, ct));
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteBike.Command { UserId = userId }, ct));
		}
	}
}