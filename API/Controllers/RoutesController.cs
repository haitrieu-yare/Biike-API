using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Routes;
using Application.Routes.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class RoutesController : BaseApiController
	{
		// Keer, Biker, Admin
		[HttpGet]
		public async Task<IActionResult> GetAllRoutes(int page, int limit, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new ListRoutes.Query { Page = page, Limit = limit, IsAdmin = validationDto.IsAdmin }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{routeId:int}")]
		public async Task<IActionResult> GetRouteByRouteId(int routeId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new DetailRoute.Query { RouteId = routeId, IsAdmin = validationDto.IsAdmin }, ct));
		}

		// Admin
		[HttpPost]
		public async Task<IActionResult> CreateRoute(RouteCreateDto routeCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new CreateRoute.Command { RouteCreateDto = routeCreateDto }, ct));
		}

		// Admin
		[HttpPut("{routeId:int}")]
		public async Task<IActionResult> EditRoute(int routeId, RouteDto routeDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new EditRoute.Command { RouteId = routeId, RouteDto = routeDto },
				ct));
		}

		// Admin
		[HttpDelete("{routeId:int}")]
		public async Task<IActionResult> DeleteRoute(int routeId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteRoute.Command { RouteId = routeId }, ct));
		}
	}
}