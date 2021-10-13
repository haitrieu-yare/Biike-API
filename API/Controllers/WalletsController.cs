using System.Threading;
using System.Threading.Tasks;
using Application.Wallets;
using Application.Wallets.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class WalletsController : BaseApiController
	{
		// Admin
		[HttpGet]
		public async Task<IActionResult> GetAllWallets(int page, int limit, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};
			
			return HandleResult(await Mediator.Send(new ListWallets.Query { Page = page, Limit = limit }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("users/{userId:int}")]
		public async Task<IActionResult> GetAllWalletsByUserId(int page, int limit, int userId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantStringApi.DidNotHavePermissionToAccess);

			return HandleResult(await Mediator.Send(
				new ListWalletsByUserId.Query { Page = page, Limit = limit, UserId = userId }, ct));
		}

		// Admin
		[HttpGet("{walletId:int}")]
		public async Task<IActionResult> GetWalletByWalletId(int walletId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};
			
			return HandleResult(await Mediator.Send(new DetailWallet.Query { WalletId = walletId }, ct));
		}

		// Admin
		[HttpPost]
		public async Task<IActionResult> CreateWallet(WalletCreateDto walletCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};
			
			return HandleResult(await Mediator.Send(
				new CreateWallet.Command { WalletCreateDto = walletCreateDto }, ct));
		}

		// Admin
		[HttpPut("{walletId:int}")]
		public async Task<IActionResult> EditWalletByWalletId(int walletId,
			WalletDto newWalletDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};
			
			return HandleResult(await Mediator.Send(
				new EditWallet.Command { WalletId = walletId, NewWalletDto = newWalletDto }, ct));
		}

		// Admin
		[HttpDelete("{walletId:int}")]
		public async Task<IActionResult> DeleteWalletByWalletId(int walletId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};
				
			return HandleResult(await Mediator.Send(new DeleteWallet.Command { WalletId = walletId }, ct));
		}
	}
}