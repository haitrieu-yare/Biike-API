using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Redemptions;
using Application.Redemptions.DTOs;

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

		[HttpGet("{userId}/full")]
		public async Task<IActionResult> GetAllRedemptionsAndVouchers(int userId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListUserRedemptionAndVoucher.Query { UserId = userId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateRedemption(RedemptionCreateDTO redemptionCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateRedemption.Command { RedemptionCreateDTO = redemptionCreateDTO }, ct));
		}

		[HttpPut("{walletId}")]
		public async Task<IActionResult> EditUsageRedemption(int walletId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditUsageRedemption.Command { WalletId = walletId }, ct));
		}
	}
}