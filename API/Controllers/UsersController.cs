using System;
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
		[Authorized(RoleStatus.Admin)]
		[HttpGet]
		public async Task<IActionResult> GetAllUser(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListAllUsers.Query { Page = page, Limit = limit }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpGet("{userId:int}/profile")]
		public async Task<IActionResult> GetUserSelfProfile(int userId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's profile.");

			return HandleResult(await Mediator.Send(new DetailSelfUser.Query { UserId = userId }, ct));
		}

		[HttpGet("{userId:int}")]
		public async Task<IActionResult> GetUserProfile(int userId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int) RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailUser.Query { IsAdmin = isAdmin, UserId = userId }, ct));
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

		[AllowAnonymous]
		[HttpPut("{userId:int}/activation")]
		public async Task<IActionResult> VerifyUser(
			int userId, UserActivationDto userActivationDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ModifyAccountActivation.Command { UserId = userId, UserActivationDto = userActivationDto }, ct));
		}

		[AllowAnonymous]
		[HttpGet("{userId:int}/activation")]
		public async Task<IActionResult> GetUserActivation(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckAccountActivation.Query { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPut("{userId:int}/profile")]
		public async Task<IActionResult> EditUserProfile(int userId,
			UserProfileEditDto userProfileEditDto, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's profile.");

			return HandleResult(await Mediator.Send(
				new EditProfile.Command { UserId = userId, UserProfileEditDto = userProfileEditDto }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPut("role")]
		public async Task<IActionResult> EditUserRole(CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			return HandleResult(await Mediator.Send(
				new EditRole.Command { UserId = validationDto.UserRequestId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{userId:int}")]
		public async Task<IActionResult> EditUserStatus(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditStatus.Command { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPut("{userId:int}/login-device")]
		public async Task<IActionResult> EditUserLoginDevice(int userId,
			UserLoginDeviceDto userLoginDeviceDto, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's info.");

			var userAuthTimeClaim = HttpContext.User.FindFirst(c => c.Type.Equals("auth_time"));
			string? authTimeString = userAuthTimeClaim?.Value;

			if (string.IsNullOrEmpty(authTimeString))
				return BadRequest("Can't get authentication time of the who send the request.");

			int authTime = int.Parse(authTimeString);
			DateTime beginningTime = DateTime.UnixEpoch;
			var currentTimeUtc7 = beginningTime.AddSeconds(authTime).AddHours(7);

			userLoginDeviceDto.LastTimeLogin = currentTimeUtc7;

			return HandleResult(await Mediator.Send(
				new EditLoginDevice.Command { UserId = userId, UserLoginDeviceDto = userLoginDeviceDto }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{userId:int}")]
		public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteUser.Command { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("deleteFirebase")]
		public async Task<IActionResult> DeleteAllFirebaseUsers(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteFireBaseUser.Command(), ct));
		}
	}
}