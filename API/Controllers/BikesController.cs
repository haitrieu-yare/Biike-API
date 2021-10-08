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
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllBikes(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0)
			{
				return Unauthorized(ConstantString.CouldNotGetUserRole);
			}
			else if (role != (int)RoleStatus.Admin)
			{
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };
			}

			return HandleResult(await Mediator.Send(new ListBikes.Query { Page = page, Limit = limit }, ct));
		}

		// Admin
		[HttpGet("{bikeId:int}")]
		public async Task<IActionResult> GetBikeByBikeId(int bikeId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0)
			{
				return Unauthorized(ConstantString.CouldNotGetUserRole);
			}
			else if (role != (int)RoleStatus.Admin)
			{
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };
			}

			return HandleResult(await Mediator.Send(new DetailBikeByBikeId.Query { BikeId = bikeId }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("users/{userId:int}")]
		public async Task<IActionResult> GetBikeByUserId(int userId, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return new ObjectResult(ConstantString.DidNotHavePermissionToAccess) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new DetailBikeByUserId.Query
				{
					IsAdmin = validationDto.IsAdmin,
					UserId = validationDto.UserRequestId
				}, ct));
		}

		// Keer
		[HttpPost]
		public async Task<IActionResult> CreateBike(BikeCreateDTO bikeCreateDTO, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0)
			{
				return Unauthorized(ConstantString.CouldNotGetUserRole);
			}
			else if (role != (int)RoleStatus.Keer)
			{
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString())) { StatusCode = 403 };
			}

			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, bikeCreateDTO.UserId);

			if (!validationDto.IsUserFound)
				return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new CreateBike.Command { BikeCreateDTO = bikeCreateDTO }, ct));
		}

		// Biker
		[HttpPost("bikeReplacement")]
		public async Task<IActionResult> ReplaceBike(BikeCreateDTO bikeCreateDTO, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0)
			{
				return Unauthorized(ConstantString.CouldNotGetUserRole);
			}
			else if (role != (int)RoleStatus.Biker)
			{
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };
			}

			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, bikeCreateDTO.UserId);

			if (!validationDto.IsUserFound)
				return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new ReplaceBike.Command { BikeCreateDTO = bikeCreateDTO }, ct));
		}

		// Biker
		[HttpDelete("{userId}:int")]
		public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0)
			{
				return Unauthorized(ConstantString.CouldNotGetUserRole);
			}
			else if (role != (int)RoleStatus.Biker)
			{
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };
			}

			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return new ObjectResult(ConstantString.DidNotHavePermissionToMakeRequest) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteBike.Command { UserId = userId }, ct));
		}
	}
}