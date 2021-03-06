using System.Threading;
using System.Threading.Tasks;
using Application;
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
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new WalletList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetAllWalletsByUserId(int page, int limit, int userId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new WalletListByUserId.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("users/{userId:int}/expiration")]
        public async Task<IActionResult> GetUpcomingExpiredWalletByUserId(int userId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(new UpcomingExpiredWallet.Query(userId), ct));
        }

        // Admin
        [HttpGet("{walletId:int}")]
        public async Task<IActionResult> GetWalletByWalletId(int walletId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new WalletDetails.Query {WalletId = walletId}, ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateWallet(WalletCreationDto walletCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new WalletCreation.Command {WalletCreationDto = walletCreationDto}, ct));
        }

        // Admin
        [HttpPut("{walletId:int}")]
        public async Task<IActionResult> EditWalletByWalletId(int walletId,
            WalletDto newWalletDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new WalletEdit.Command {WalletId = walletId, NewWalletDto = newWalletDto}, ct));
        }

        // Admin
        [HttpDelete("{walletId:int}")]
        public async Task<IActionResult> DeleteWalletByWalletId(int walletId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new WalletDeletion.Command {WalletId = walletId}, ct));
        }
    }
}