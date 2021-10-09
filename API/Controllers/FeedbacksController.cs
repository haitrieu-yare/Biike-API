using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Feedbacks;
using Application.Feedbacks.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class FeedbacksController : BaseApiController
	{
		[Authorized(RoleStatus.Admin)]
		[HttpGet]
		public async Task<IActionResult> GetTripAllFeedBacks(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListAllFeedbacks.Query { Page = page, Limit = limit }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpGet("{feedbackId:int}")]
		public async Task<IActionResult> GetTripFeedBackById(int feedbackId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailFeedback.Query { FeedbackId = feedbackId }, ct));
		}

		[HttpGet("trips/{tripId:int}")]
		public async Task<IActionResult> GetTripFeedBacks(int tripId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			return HandleResult(await Mediator.Send(
				new ListFeedbacksByTrip.Query
				{
					IsAdmin = validationDto.IsAdmin,
					TripId = tripId,
					UserRequestId = validationDto.UserRequestId,
				}, ct));
		}

		[Authorized(RoleStatus.Keer, RoleStatus.Biker)]
		[HttpPost]
		public async Task<IActionResult> CreateFeedBack(FeedbackCreateDto feedbackCreateDto, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, feedbackCreateDto.UserId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of feedback.");

			return HandleResult(await Mediator.Send(
				new CreateFeedback.Command { FeedbackCreateDto = feedbackCreateDto }, ct));
		}
	}
}