using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.PointHistory;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class PointHistoryController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllPointHistory(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new PointHistoryList.Query(page, limit), ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetPointHistoryListByUserId(int userId, int page, int limit,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new PointHistoryListByUserId.Query(page, limit, userId), ct));
        }
    }
}