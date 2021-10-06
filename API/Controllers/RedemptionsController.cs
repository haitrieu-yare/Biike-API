using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Redemptions;
using Application.Redemptions.DTOs;

namespace API.Controllers
{
	[Authorize]
	public class RedemptionsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetAllRedemptions(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListRedemption.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{redemptionId}")]
		public async Task<IActionResult> GetRedemption(int redemptionId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailRedemption.Query { RedemptionId = redemptionId }, ct));
		}

		[HttpGet("users/{userId}")]
		public async Task<IActionResult> GetUserRedemptions(
			int userId, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListUserRedemption.Query { Page = page, Limit = limit, UserId = userId }, ct));
		}

		[HttpGet("users/{userId}/full")]
		public async Task<IActionResult> GetAllRedemptionsAndVouchers(
			int userId, int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListUserRedemptionAndVoucher.Query { Page = page, Limit = limit, UserId = userId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRedemption(RedemptionCreateDTO redemptionCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateRedemption.Command { RedemptionCreateDTO = redemptionCreateDTO }, ct));
		}

		[HttpPut("{redemptionId}")]
		public async Task<IActionResult> EditUsageRedemption(int redemptionId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditUsageRedemption.Command { RedemptionId = redemptionId }, ct));
		}
	}
}