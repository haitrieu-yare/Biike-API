using System.Threading;
using System.Threading.Tasks;
using Application.Intimacies;
using Application.Intimacies.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class IntimaciesController : BaseApiController
	{
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllIntimacies(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new ListIntimacies.Query { Page = page, Limit = limit }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{userOneId:int}")]
		public async Task<IActionResult> GetIntimaciesByUserId(int userOneId, int page, int limit, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) 
				return new ObjectResult(ConstantString.DidNotHavePermissionToAccess) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new ListIntimaciesByUserId.Query { Page = page, Limit = limit, UserOneId = userOneId }, ct));
		}

		// Keer, Biker
		[HttpPost]
		public async Task<IActionResult> CreateIntimacy(IntimacyCreateEditDto intimacyCreateEditDto,
			CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, intimacyCreateEditDto.UserOneId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new CreateIntimacy.Command { IntimacyCreateEditDto = intimacyCreateEditDto }, ct));
		}

		// Keer, Biker
		[HttpPut]
		public async Task<IActionResult> EditIntimacies(IntimacyCreateEditDto intimacyCreateEditDto,
			CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, intimacyCreateEditDto.UserOneId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new EditIntimacy.Command { IntimacyCreateEditDto = intimacyCreateEditDto }, ct));
		}
	}
}