using System.Threading;
using System.Threading.Tasks;
using Application.Feedbacks.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class FeedbacksController : BaseApiController
	{
		[HttpPost]
		public async Task<IActionResult> CreateFeedBack(FeedbackCreateDTO feedbackCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command { FeedbackCreateDTO = feedbackCreateDTO }, ct));
		}
	}
}