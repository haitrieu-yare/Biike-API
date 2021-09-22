using System.Threading;
using System.Threading.Tasks;
using Application.AppUsers;
using Application.AppUsers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class UsersController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllUser(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpPut("{id}/profile")]
		public async Task<IActionResult> EditUserProfile(int id,
			AppUserProfileEditDTO appUserProfileEditDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditProfile.Command
			{ Id = id, AppUserProfileEditDTO = appUserProfileEditDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditUserStatus(int id, int newStatus, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditStatus.Command
			{ Id = id, NewStatus = newStatus }, ct));
		}

		[HttpPut("{id}/login-device")]
		public async Task<IActionResult> EditUserLoginDevice(int id,
			AppUserLoginDeviceDTO appUserLoginDeviceDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditLoginDevice.Command
			{ Id = id, AppUserLoginDeviceDTO = appUserLoginDeviceDTO }, ct));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Delete.Command { Id = id }, ct));
		}
	}
}