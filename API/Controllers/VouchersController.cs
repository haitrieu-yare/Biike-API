using System.Threading;
using System.Threading.Tasks;
using Application.Vouchers;
using Application.Vouchers.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class VouchersController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVouchers(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListVouchers.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{voucherId}")]
		public async Task<IActionResult> GetVoucher(int voucherId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailVoucher.Query { VoucherId = voucherId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateVoucher(VoucherCreateDto voucherCreateDto,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateVoucher.Command { VoucherCreateDto = voucherCreateDto }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{voucherId}")]
		public async Task<IActionResult> EditVoucher(int voucherId, VoucherEditDto newVoucher, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditVoucher.Command { VoucherId = voucherId, NewVoucher = newVoucher }, ct));
		}
	}
}