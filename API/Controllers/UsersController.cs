using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Users;
using Application.Users.DTOs;

namespace API.Controllers
{
	public class UsersController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllUser(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}/profile")]
		public async Task<IActionResult> GetUserSelfProfile(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailSelf.Query { Id = id }, ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserProfile(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPost("checkExist")]
		public async Task<IActionResult> CheckExistUser(UserExistDTO userExistDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CheckExistUser.Command { UserExistDTO = userExistDTO }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> SignUp(UserCreateDTO userCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateUser.Command { UserCreateDTO = userCreateDTO }, ct));
		}

		[HttpPut("{id}/profile")]
		public async Task<IActionResult> EditUserProfile(int id,
			UserProfileEditDTO userProfileEditDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditProfile.Command { Id = id, UserProfileEditDTO = userProfileEditDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditUserStatus(int id, int newStatus, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditStatus.Command { Id = id, NewStatus = newStatus }, ct));
		}

		[HttpPut("{id}/login-device")]
		public async Task<IActionResult> EditUserLoginDevice(int id,
			UserLoginDeviceDTO userLoginDeviceDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditLoginDevice.Command
			{ Id = id, UserLoginDeviceDTO = userLoginDeviceDTO }, ct));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Delete.Command { Id = id }, ct));
		}

		[HttpDelete("deleteFirebase")]
		public async Task<IActionResult> DeleteAllFirebaseUsers(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteFireBaseUser.Command(), ct));
		}
	}
}