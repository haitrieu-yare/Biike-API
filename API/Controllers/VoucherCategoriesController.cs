using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.VoucherCategories;
using Application.VoucherCategories.DTOs;

namespace API.Controllers
{
	public class VoucherCategoriesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVoucherCategories(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListVoucherCategory.Query(), ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateVoucherCategory(VoucherCategoryCreateDTO voucherCategoryCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateVoucherCategory.Command { VoucherCategoryCreateDTO = voucherCategoryCreateDTO }, ct));
		}

		[HttpPut("{voucherCategoryId}")]
		public async Task<IActionResult> EditVoucherCategory(int voucherCategoryId, VoucherCategoryDTO newVoucherCategoryDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new EditVoucherCategory.Command
			{ VoucherCategoryId = voucherCategoryId, NewVoucherCategoryDTO = newVoucherCategoryDTO }, ct));
		}
	}
}