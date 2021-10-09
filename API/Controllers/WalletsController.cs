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
        [Authorized(RoleStatus.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllWallets(int page, int limit, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListWallets.Query {Page = page, Limit = limit}, ct));
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetAllWalletsByUserId(int page, int limit, int userId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of wallet.");

            return HandleResult(await Mediator.Send(
                new ListWalletsByUserId.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetWalletByWalletId(int walletId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailWallet.Query {WalletId = walletId}, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateWallet(WalletCreateDto walletCreateDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(
                new CreateWallet.Command {WalletCreateDto = walletCreateDto}, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpPut("{walletId}")]
        public async Task<IActionResult> EditWalletByWalletId(int walletId,
            WalletDto newWalletDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(
                new EditWallet.Command {WalletId = walletId, NewWalletDto = newWalletDto}, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpDelete("{walletId}")]
        public async Task<IActionResult> DeleteWalletByWalletId(int walletId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DeleteWallet.Command {WalletId = walletId}, ct));
        }
    }
}