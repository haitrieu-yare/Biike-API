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
		// Keer, Biker, Admin
		[HttpGet]
		public async Task<IActionResult> GetVouchers(int page, int limit, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(new ListVouchers.Query { Page = page, Limit = limit }, ct));
		}

		// Keer, Biker, Admin
		[HttpGet("{voucherId:int}")]
		public async Task<IActionResult> GetVoucher(int voucherId, CancellationToken ct)
		{
			ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

			if (!validationDto.IsUserFound) return BadRequest(ConstantStringApi.CouldNotGetIdOfUserSentRequest);

			return HandleResult(await Mediator.Send(new DetailVoucher.Query { VoucherId = voucherId }, ct));
		}

		// Admin
		[HttpPost]
		public async Task<IActionResult> CreateVoucher(VoucherCreateDto voucherCreateDto,
			CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new CreateVoucher.Command { VoucherCreateDto = voucherCreateDto }, ct));
		}

		// Admin
		[HttpPut("{voucherId:int}")]
		public async Task<IActionResult> EditVoucher(int voucherId, VoucherEditDto newVoucher, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantStringApi.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantStringApi.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new EditVoucher.Command { VoucherId = voucherId, NewVoucher = newVoucher }, ct));
		}
	}
}