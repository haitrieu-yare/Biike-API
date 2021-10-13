using System.Threading;
using System.Threading.Tasks;
using Application.Redemptions;
using Application.Redemptions.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class RedemptionsController : BaseApiController
	{
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllRedemptions(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new ListRedemption.Query { Page = page, Limit = limit }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{redemptionId:int}")]
		public async Task<IActionResult> GetRedemptionByRedemptionId(int redemptionId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new DetailRedemption.Query
				{
					RedemptionId = redemptionId,
					UserRequestId = validationDto.UserRequestId,
					IsAdmin = validationDto.IsAdmin
				}, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{redemptionId:int}/full")]
		public async Task<IActionResult> GetRedemptionFullByRedemptionId(int redemptionId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new DetailRedemptionFull.Query
				{
					RedemptionId = redemptionId,
					UserRequestId = validationDto.UserRequestId,
					IsAdmin = validationDto.IsAdmin
				}, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("users/{userId:int}")]
		public async Task<IActionResult> GetRedemptionsByUserId(int userId, int page, int limit, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new ListUserRedemption.Query { Page = page, Limit = limit, UserId = userId }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("users/{userId:int}/full")]
		public async Task<IActionResult> GetAllRedemptionsAndVouchers(int userId, int page, int limit,
			CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new ListUserRedemptionAndVoucher.Query { Page = page, Limit = limit, UserId = userId }, ct));
		}

		// Keer, Biker
		[HttpPost]
		public async Task<IActionResult> CreateRedemption(RedemptionCreateDto redemptionCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, redemptionCreateDto.UserId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new CreateRedemption.Command { RedemptionCreateDto = redemptionCreateDto }, ct));
		}

		// Keer, Biker
		[HttpPut("{redemptionId:int}")]
		public async Task<IActionResult> EditUsageRedemption(int redemptionId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new EditUsageRedemption.Command
				{
					RedemptionId = redemptionId, UserRequestId = validationDto.UserRequestId
				}, ct));
		}
	}
}