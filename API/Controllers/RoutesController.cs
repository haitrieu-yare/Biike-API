using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Routes;
using Application.Routes.DTOs;

namespace API.Controllers
{
	public class RoutesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllRoutes(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListRoute.Query(), ct));
		}

		[HttpGet("{routeId}")]
		public async Task<IActionResult> GetRouteByRouteId(int routeId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailRoute.Query { RouteId = routeId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRoute(RouteCreateDTO routeCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateRoute.Command { RouteCreateDTO = routeCreateDTO }, ct));
		}

		[HttpPut("{routeId}")]
		public async Task<IActionResult> EditRoute(int routeId, RouteDTO routeDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditRoute.Command { RouteId = routeId, RouteDTO = routeDTO }, ct));
		}

		[HttpDelete("{routeId}")]
		public async Task<IActionResult> DeleteRoute(int routeId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteRoute.Command { RouteId = routeId }, ct));
		}
	}
}