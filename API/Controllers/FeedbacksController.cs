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
		[HttpGet]
		public async Task<IActionResult> GetTripAllFeedBacks(CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new ListAllFeedbacks.Query { IsAdmin = isAdmin }, ct));
		}

		[HttpGet("{feedbackId}")]
		public async Task<IActionResult> GetTripFeedBackById(int feedbackId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new DetailFeedback.Query { IsAdmin = isAdmin, FeedbackId = feedbackId }, ct));
		}

		[HttpGet("trips/{tripId}")]
		public async Task<IActionResult> GetTripFeedBacks(int tripId, CancellationToken ct)
		{
			bool isAdmin = HttpContext.User.IsInRole(((int)RoleStatus.Admin).ToString());
			return HandleResult(await Mediator.Send(
				new ListFeedbacksByTrip.Query { IsAdmin = isAdmin, TripId = tripId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateFeedBack(FeedbackCreateDTO feedbackCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateFeedback.Command { FeedbackCreateDTO = feedbackCreateDTO }, ct));
		}
	}
}