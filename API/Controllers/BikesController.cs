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
		[Authorized(RoleStatus.Admin)]
		[HttpGet]
		public async Task<IActionResult> GetAllBikes(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListBikes.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("users/{userId}")]
		public async Task<IActionResult> GetBikeByUserId(int userId, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.CheckRequestUserId(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return Forbid();

			return HandleResult(await Mediator.Send(
				new DetailBikeByUserId.Query
				{
					IsAdmin = validationDto.IsAdmin,
					UserId = userId,
					UserRequestId = validationDto.UserRequestId
				}, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpGet("{bikeId}")]
		public async Task<IActionResult> GetBikeByBikeId(int bikeId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailBikeByBikeId.Query { BikeId = bikeId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateBike(BikeCreateDTO bikeCreateDTO, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.CheckRequestUserId(HttpContext, bikeCreateDTO.UserId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of bike.");

			return HandleResult(await Mediator.Send(
				new CreateBike.Command { BikeCreateDTO = bikeCreateDTO }, ct));
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.CheckRequestUserId(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of bike.");

			return HandleResult(await Mediator.Send(new DeleteBike.Command { UserId = userId }, ct));
		}
	}
}