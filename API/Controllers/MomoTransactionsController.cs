using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.MomoTransactions;
using Application.MomoTransactions.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MomoTransactionsController : BaseApiController
    {
        // Admin
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMomoTransactions(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new MomoTransactionList.Query(page, limit), ct));
        }
        
        // Keer, Biker
        [HttpPost]
        public async Task<IActionResult> CreateMomoTransaction(MomoTransactionCreationDto momoTransactionCreationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new MomoTransactionCreation.Command(momoTransactionCreationDto, validationDto.UserRequestId), ct));
        }
    }
}