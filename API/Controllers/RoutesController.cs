using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Routes;
using Application.Routes.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class RoutesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllRoutes(int page, int limit, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new ListRoutes.Query { Page = page, Limit = limit, IsAdmin = isAdmin }, ct));
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
		public async Task<IActionResult> CreateRoute(RouteCreateDto routeCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateRoute.Command { RouteCreateDto = routeCreateDto }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{routeId}")]
		public async Task<IActionResult> EditRoute(int routeId, RouteDto routeDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditRoute.Command { RouteId = routeId, RouteDto = routeDto }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{routeId}")]
		public async Task<IActionResult> DeleteRoute(int routeId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteRoute.Command { RouteId = routeId }, ct));
		}
	}
}