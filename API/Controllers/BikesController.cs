using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Bikes;
using Application.Bikes.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class BikesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllBikes(CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(new ListBikes.Query { IsAdmin = isAdmin }, ct));
		}

		[HttpGet("users/{userId}")]
		public async Task<IActionResult> GetBikeByUserId(int userId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailBikeByUserId.Query { IsAdmin = isAdmin, UserId = userId }, ct));
		}

		[HttpGet("{bikeId}")]
		public async Task<IActionResult> GetBikeByBikeId(int bikeId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailBikeByBikeId.Query { IsAdmin = isAdmin, BikeId = bikeId }, ct));
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