using System.Threading;
using System.Threading.Tasks;
using Application.Redemptions;
using Application.Redemptions.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class RedemptionsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllRedemptions(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListRedemption.Query(), ct));
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUserRedemptions(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListUserRedemption.Query { UserId = userId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRedemption(RedemptionCreateDTO redemptionCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateRedemption.Command
			{ RedemptionCreateDTO = redemptionCreateDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditUsageRedemption(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditUsageRedemption.Command { Id = id }, ct));
		}
	}
}