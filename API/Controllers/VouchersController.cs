using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Vouchers;
using Application.Vouchers.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class VouchersController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVouchers(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListVouchers.Query(), ct));
		}

		[HttpGet("{voucherId}")]
		public async Task<IActionResult> GetVoucher(int voucherId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailVoucher.Query { VoucherId = voucherId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateVoucher(VoucherCreateDTO voucherCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateVoucher.Command { VoucherCreateDTO = voucherCreateDTO }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{voucherId}")]
		public async Task<IActionResult> EditVoucher(int voucherId, VoucherEditDTO newVoucher, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditVoucher.Command { VoucherId = voucherId, NewVoucher = newVoucher }, ct));
		}
	}
}