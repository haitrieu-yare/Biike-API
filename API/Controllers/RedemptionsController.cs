using System.Threading;
using System.Threading.Tasks;
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
        [Authorized(RoleStatus.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllRedemptions(int page, int limit, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListRedemption.Query {Page = page, Limit = limit}, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpGet("{redemptionId:int}")]
        public async Task<IActionResult> GetRedemption(int redemptionId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailRedemption.Query {RedemptionId = redemptionId}, ct));
        }

        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetUserRedemptions(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of redemption.");

            return HandleResult(await Mediator.Send(
                new ListUserRedemption.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        [HttpGet("users/{userId:int}/full")]
        public async Task<IActionResult> GetAllRedemptionsAndVouchers(
            int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of redemption.");

            return HandleResult(await Mediator.Send(
                new ListUserRedemptionAndVoucher.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRedemption(RedemptionCreateDto redemptionCreateDto,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, redemptionCreateDto.UserId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of redemption.");

            return HandleResult(await Mediator.Send(
                new CreateRedemption.Command {RedemptionCreateDto = redemptionCreateDto}, ct));
        }

        [HttpPut("{redemptionId}")]
        public async Task<IActionResult> EditUsageRedemption(int redemptionId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            return HandleResult(await Mediator.Send(
                new EditUsageRedemption.Command
                {
                    RedemptionId = redemptionId,
                    UserRequestId = validationDto.UserRequestId
                }, ct));
        }
    }
}