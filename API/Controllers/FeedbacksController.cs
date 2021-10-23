using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Feedbacks;
using Application.Feedbacks.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FeedbacksController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllFeedBacks(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new FeedbackList.Query {Page = page, Limit = limit}, ct));
        }

        // Admin
        [HttpGet("{feedbackId:int}")]
        public async Task<IActionResult> GetFeedBackById(int feedbackId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new FeedbackDetails.Query {FeedbackId = feedbackId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("trips/{tripId:int}")]
        public async Task<IActionResult> GetFeedBacksByTripId(int tripId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new FeedbackListByTripId.Query
                {
                    IsAdmin = validationDto.IsAdmin, TripId = tripId, UserRequestId = validationDto.UserRequestId
                }, ct));
        }

        // Keer, Biker
        [HttpPost]
        public async Task<IActionResult> CreateFeedBack(FeedbackCreationDto feedbackCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, feedbackCreationDto.UserId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new FeedbackCreation.Command {FeedbackCreationDto = feedbackCreationDto}, ct));
        }
    }
}