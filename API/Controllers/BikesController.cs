using System.Threading;
using System.Threading.Tasks;
using Application.Bikes;
using Application.Bikes.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class BikesController : BaseApiController
	{
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllBikes(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new ListBikes.Query { Page = page, Limit = limit }, ct));
		}

		// Admin
		[HttpGet("{bikeId:int}")]
		public async Task<IActionResult> GetBikeByBikeId(int bikeId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DetailBikeByBikeId.Query { BikeId = bikeId }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("users/{userId:int}")]
		public async Task<IActionResult> GetBikeByUserId(int userId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return new ObjectResult(ConstantStringApi.DidNotHavePermissionToAccess) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new DetailBikeByUserId.Query { IsAdmin = validationDto.IsAdmin, UserId = validationDto.UserRequestId },
				ct));
		}

		// Keer
		[HttpPost]
		public async Task<IActionResult> CreateBike(BikeCreateDto bikeCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Keer.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikeCreateDto.UserId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantStringApi.NotSameUserId);

			return HandleResult(await Mediator.Send(new CreateBike.Command { BikeCreateDto = bikeCreateDto }, ct));
		}

		// Biker
		[HttpPost("bikeReplacement")]
		public async Task<IActionResult> ReplaceBike(BikeCreateDto bikeCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikeCreateDto.UserId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantStringApi.NotSameUserId);

			return HandleResult(await Mediator.Send(new ReplaceBike.Command { BikeCreateDto = bikeCreateDto }, ct));
		}

		// Biker
		[HttpDelete("{userId:int}")]
		public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return new ObjectResult(ConstantStringApi.DidNotHavePermissionToMakeRequest) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteBike.Command { UserId = userId }, ct));
		}
	}
}