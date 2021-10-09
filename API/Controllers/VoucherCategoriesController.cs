using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.VoucherCategories;
using Application.VoucherCategories.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class VoucherCategoriesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVoucherCategories(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListVoucherCategory.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{voucherCategoryId}")]
		public async Task<IActionResult> GetVoucherCategory(int voucherCategoryId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailVoucherCategory.Query { VoucherCategoryId = voucherCategoryId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateVoucherCategory(
			VoucherCategoryCreateDto voucherCategoryCreateDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateVoucherCategory.Command { VoucherCategoryCreateDto = voucherCategoryCreateDto }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpPut("{voucherCategoryId}")]
		public async Task<IActionResult> EditVoucherCategory(
			int voucherCategoryId, VoucherCategoryDto newVoucherCategoryDto, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditVoucherCategory.Command
				{
					VoucherCategoryId = voucherCategoryId,
					NewVoucherCategoryDto = newVoucherCategoryDto
				}, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpDelete("{voucherCategoryId}")]
		public async Task<IActionResult> DeleteVoucherCategory(int voucherCategoryId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DeleteVoucherCategory.Command { VoucherCategoryId = voucherCategoryId }, ct));
		}
	}
}