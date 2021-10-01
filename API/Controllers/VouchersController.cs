using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Vouchers;
using Application.Vouchers.DTOs;

namespace API.Controllers
{
	public class VouchersController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVouchers(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListVouchers.Query(), ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateVoucher(VoucherCreateDTO voucherCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new CreateVoucher.Command
			{ VoucherCreateDTO = voucherCreateDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditVoucher(int id, VoucherEditDTO newVoucher,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditVoucher.Command
			{ Id = id, NewVoucher = newVoucher }, ct));
		}
	}
}