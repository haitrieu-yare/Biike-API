using System.Threading;
using System.Threading.Tasks;
using Application.Bikes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class BikesController : BaseApiController
	{
		[HttpGet("{userId}")]
		public async Task<IActionResult> GetBike(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { UserId = userId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateBike(BikeDTO bikeDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { BikeDTO = bikeDTO }, ct));
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Delete.Command { UserId = userId }, ct));
		}
	}
}