using System.Threading;
using System.Threading.Tasks;
using Application.Feedbacks;
using Application.Feedbacks.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class FeedbacksController : BaseApiController
	{
		[HttpPost]
		public async Task<IActionResult> CreateFeedBack(FeedbackDTO feedbackDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { FeedbackDTO = feedbackDTO }, ct));
		}

		[HttpGet("{tripId}")]
		public async Task<IActionResult> GetTripFeedBacks(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query { TripId = tripId }, ct));
		}
	}
}