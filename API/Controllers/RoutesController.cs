using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Routes;
using Application.Routes.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class RoutesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllRoutes(CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(new ListRoute.Query { IsAdmin = isAdmin }, ct));
		}

		[HttpGet("{routeId}")]
		public async Task<IActionResult> GetRouteByRouteId(int routeId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailRoute.Query { IsAdmin = isAdmin, RouteId = routeId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateRoute(RouteCreateDTO routeCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateRoute.Command { RouteCreateDTO = routeCreateDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{routeId}")]
		public async Task<IActionResult> EditRoute(int routeId, RouteDTO routeDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditRoute.Command { RouteId = routeId, RouteDTO = routeDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{routeId}")]
		public async Task<IActionResult> DeleteRoute(int routeId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteRoute.Command { RouteId = routeId }, ct));
		}
	}
}