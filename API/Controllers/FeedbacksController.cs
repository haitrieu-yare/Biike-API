using System.Threading;
using System.Threading.Tasks;
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
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new ListAllFeedbacks.Query { Page = page, Limit = limit }, ct));
		}

		// Admin
		[HttpGet("{feedbackId:int}")]
		public async Task<IActionResult> GetFeedBackById(int feedbackId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(new DetailFeedback.Query { FeedbackId = feedbackId }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("trips/{tripId:int}")]
		public async Task<IActionResult> GetFeedBacksByTripId(int tripId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(
				new ListFeedbacksByTrip.Query
				{
					IsAdmin = validationDto.IsAdmin, TripId = tripId, UserRequestId = validationDto.UserRequestId
				}, ct));
		}

		// Keer, Biker
		[HttpPost]
		public async Task<IActionResult> CreateFeedBack(FeedbackCreateDto feedbackCreateDto, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
				                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) { StatusCode = 403 };

			ValidationDto validationDto = ControllerUtils.Validate(HttpContext, feedbackCreateDto.UserId);

			if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

			if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

			return HandleResult(await Mediator.Send(
				new CreateFeedback.Command { FeedbackCreateDto = feedbackCreateDto }, ct));
		}
	}
}