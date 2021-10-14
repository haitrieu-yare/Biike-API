using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Users;
using Application.Users.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class UsersController : BaseApiController
	{
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllUser(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new ListAllUsers.Query { Page = page, Limit = limit }, ct));
		}

		// Keer, Biker
		[HttpGet("{userId:int}/profile")]
		public async Task<IActionResult> GetUserSelfProfile(int userId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantStringApi.DidNotHavePermissionToAccess);

			return HandleResult(await Mediator.Send(new DetailSelfUser.Query { UserId = userId }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{userId:int}")]
		public async Task<IActionResult> GetUserProfile(int userId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new DetailUser.Query { IsAdmin = validationDto.IsAdmin, UserId = userId }, ct));
		}

		[AllowAnonymous]
		[HttpPost("checkExist")]
		public async Task<IActionResult> CheckExistUser(UserExistDto userExistDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckExistUser.Command { UserExistDto = userExistDto }, ct));
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> SignUp(UserCreateDto userCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateUser.Command { UserCreateDto = userCreateDto }, ct));
		}

		// TODO: When a user can modify account activation?
		[AllowAnonymous]
		[HttpPut("{userId:int}/activation")]
		public async Task<IActionResult> ModifyAccountActivation(
			int userId, UserActivationDto userActivationDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ModifyAccountActivation.Command { UserId = userId, UserActivationDto = userActivationDto }, ct));
		}

		[AllowAnonymous]
		[HttpGet("{userId:int}/activation")]
		public async Task<IActionResult> CheckAccountActivation(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckAccountActivation.Query { UserId = userId }, ct));
		}

		// Keer, Biker
		[HttpPut("{userId:int}/profile")]
		public async Task<IActionResult> EditUserProfile(int userId,
			UserProfileEditDto userProfileEditDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized)
				return BadRequest(ConstantStringApi.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new EditProfile.Command { UserId = userId, UserProfileEditDto = userProfileEditDto }, ct));
		}

		// Keer, Biker
		[HttpPut("role")]
		public async Task<IActionResult> EditUserRole(CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new EditRole.Command { UserId = validationDto.UserRequestId }, ct));
		}

		// Admin
		[HttpPut("{userId:int}")]
		public async Task<IActionResult> EditUserStatus(int userId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new EditStatus.Command { UserId = userId }, ct));
		}

		// Keer, Biker
		[HttpPut("{userId:int}/login-device")]
		public async Task<IActionResult> EditUserLoginDevice(int userId,
			UserLoginDeviceDto userLoginDeviceDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantStringApi.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantStringApi.NotSameUserId);

			Claim? userAuthTimeClaim = HttpContext.User.FindFirst(c => c.Type.Equals("auth_time"));
			string? authTimeString = userAuthTimeClaim?.Value;

			if (string.IsNullOrEmpty(authTimeString))
				return BadRequest("Can't get authentication time of the who send the request.");

			int authTime = int.Parse(authTimeString);
			DateTime beginningTime = DateTime.UnixEpoch;
			DateTime currentTimeUtc7 = beginningTime.AddSeconds(authTime).AddHours(7);

			userLoginDeviceDto.LastTimeLogin = currentTimeUtc7;

			return HandleResult(await Mediator.Send(
				new EditLoginDevice.Command { UserId = userId, UserLoginDeviceDto = userLoginDeviceDto }, ct));
		}

		// Admin
		[HttpDelete("{userId:int}")]
		public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteUser.Command { UserId = userId }, ct));
		}

		// Admin
		[HttpDelete("deleteFirebase")]
		public async Task<IActionResult> DeleteAllFirebaseUsers(CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DeleteFireBaseUser.Command(), ct));
		}
	}
}