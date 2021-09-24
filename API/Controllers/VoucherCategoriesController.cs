using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.VoucherCategories;

namespace API.Controllers
{
	public class VoucherCategoriesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetVoucherCategories(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateVoucherCategory(VoucherCategoryDTO voucherCategoryDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Create.Command
			{ VoucherCategoryDTO = voucherCategoryDTO }, ct));
		}
	}
}