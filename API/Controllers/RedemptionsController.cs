using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Redemptions;
using Application.Redemptions.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class RedemptionsController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllRedemptions(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new RedemptionList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{redemptionId:int}")]
        public async Task<IActionResult> GetRedemptionByRedemptionId(int redemptionId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new RedemptionDetails.Query
                {
                    RedemptionId = redemptionId,
                    UserRequestId = validationDto.UserRequestId,
                    IsAdmin = validationDto.IsAdmin
                }, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{redemptionId:int}/full")]
        public async Task<IActionResult> GetRedemptionFullByRedemptionId(int redemptionId,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new RedemptionDetailsFull.Query
                {
                    RedemptionId = redemptionId,
                    UserRequestId = validationDto.UserRequestId,
                    IsAdmin = validationDto.IsAdmin
                }, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetRedemptionsByUserId(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new RedemptionListByUseId.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("users/{userId:int}/full")]
        public async Task<IActionResult> GetAllRedemptionsAndVouchers(int userId, int page, int limit, bool isExpired,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new RedemptionAndVoucherListByUserId.Query
                {
                    Page = page, Limit = limit, UserId = userId, IsExpired = isExpired
                }, ct));
        }

        // Keer, Biker
        [HttpPost]
        public async Task<IActionResult> CreateRedemption(RedemptionCreationDto redemptionCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, redemptionCreationDto.UserId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new RedemptionCreation.Command {RedemptionCreationDto = redemptionCreationDto}, ct));
        }

        // Keer, Biker
        [HttpPut("{redemptionId:int}")]
        public async Task<IActionResult> EditUsageRedemption(int redemptionId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new RedemptionUsageEdit.Command
                {
                    RedemptionId = redemptionId, UserRequestId = validationDto.UserRequestId
                }, ct));
        }
    }
}