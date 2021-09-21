using System.Threading;
using System.Threading.Tasks;
using Application.Routes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class RoutesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetRoutes(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetRoute(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRoute(RouteDTO routeDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { RouteDTO = routeDto }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditRoute(int id, RouteDTO newRouteDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command { Id = id, NewRouteDTO = newRouteDto }, ct));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRoute(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Delete.Command { Id = id }, ct));
		}
	}
}