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
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateVoucher(VoucherCreateDTO voucherCreateDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command
			{ VoucherCreateDTO = voucherCreateDTO }, ct));
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> EditVoucher(int id, VoucherEditDTO newVoucher,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command
			{ Id = id, NewVoucher = newVoucher }, ct));
		}
	}
}