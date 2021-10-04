using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Wallets;
using Application.Wallets.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class WalletsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllWallets(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListWallets.Query(), ct));
		}

		[HttpGet("{walletId}")]
		public async Task<IActionResult> GetWalletByWalletId(int walletId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailWallet.Query { WalletId = walletId }, ct));
		}

		[HttpGet("users/{userId}")]
		public async Task<IActionResult> GetAllWalletsByUserId(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListWalletsByUserId.Query { UserId = userId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateWallet(WalletCreateDTO walletCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateWallet.Command { WalletCreateDTO = walletCreateDTO }, ct));
		}

		[HttpPut("{walletId}")]
		public async Task<IActionResult> EditWalletByWalletId(int walletId,
			WalletDTO newWalletDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditWallet.Command { WalletId = walletId, NewWalletDTO = newWalletDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{walletId}")]
		public async Task<IActionResult> DeleteWalletByWalletId(int walletId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DeleteWallet.Command { WalletId = walletId }, ct));
		}
	}
}