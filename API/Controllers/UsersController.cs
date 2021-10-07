using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Users;
using Application.Users.DTOs;
using Domain.Enums;

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
		[HttpGet("{userId}/profile")]
		public async Task<IActionResult> GetUserSelfProfile(int userId, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's profile.");

			return HandleResult(await Mediator.Send(new DetailSelfUser.Query { UserId = userId }, ct));
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUserProfile(int userId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailUser.Query { IsAdmin = isAdmin, UserId = userId }, ct));
		}

		[AllowAnonymous]
		[HttpPost("checkExist")]
		public async Task<IActionResult> CheckExistUser(UserExistDTO userExistDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckExistUser.Command { UserExistDTO = userExistDTO }, ct));
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> SignUp(UserCreateDTO userCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateUser.Command { UserCreateDTO = userCreateDTO }, ct));
		}

		[AllowAnonymous]
		[HttpPut("{userId}/activation")]
		public async Task<IActionResult> VerifyUser(
			int userId, UserActivationDTO userActivationDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ModifyAccountActivation.Command { UserId = userId, UserActivationDTO = userActivationDTO }, ct));
		}

		[AllowAnonymous]
		[HttpGet("{userId}/activation")]
		public async Task<IActionResult> GetUserActivation(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckAccountActivation.Query { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPut("{userId}/profile")]
		public async Task<IActionResult> EditUserProfile(int userId,
			UserProfileEditDTO userProfileEditDTO, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's profile.");

			return HandleResult(await Mediator.Send(
				new EditProfile.Command { UserId = userId, UserProfileEditDTO = userProfileEditDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{userId}")]
		public async Task<IActionResult> EditUserStatus(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditStatus.Command { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPut("{userId}/login-device")]
		public async Task<IActionResult> EditUserLoginDevice(int userId,
			UserLoginDeviceDTO userLoginDeviceDTO, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of user's info.");

			var userAuthTimeClaim = HttpContext.User.FindFirst(c => c.Type.Equals("auth_time"));
			string? authTimeString = userAuthTimeClaim?.Value;

			if (string.IsNullOrEmpty(authTimeString))
			{
				return BadRequest("Can't get authentication time of the who send the request.");
			}

			int authTime = int.Parse(authTimeString);
			DateTime BeginningTime = DateTime.UnixEpoch;
			var currentTimeUTC7 = BeginningTime.AddSeconds(authTime).AddHours(7);

			userLoginDeviceDTO.LastTimeLogin = currentTimeUTC7;

			return HandleResult(await Mediator.Send(
				new EditLoginDevice.Command { UserId = userId, UserLoginDeviceDTO = userLoginDeviceDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{userId}")]
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