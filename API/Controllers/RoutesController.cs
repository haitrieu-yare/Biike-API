using System.Threading;
using System.Threading.Tasks;
using Application.Routes;
using Application.Routes.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class RoutesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetRoutes(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListRoute.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetRoute(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailRoute.Query { Id = id }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRoute(RouteCreateDTO routeCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateRoute.Command
			{ RouteCreateDTO = routeCreateDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditRoute(int id, RouteDTO routeDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditRoute.Command
			{ Id = id, RouteDTO = routeDTO }, ct));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRoute(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteRoute.Command { Id = id }, ct));
		}
	}
}