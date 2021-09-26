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
		public async Task<IActionResult> CreateFeedBack(FeedbackCreateDTO feedbackCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new Create.Command { FeedbackCreateDTO = feedbackCreateDTO }, ct));
		}

		[HttpGet("{tripId}")]
		public async Task<IActionResult> GetTripFeedBack(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query { TripId = tripId }, ct));
		}

		[HttpGet]
		public async Task<IActionResult> GetTripFeedBacks(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListAll.Query(), ct));
		}
	}
}