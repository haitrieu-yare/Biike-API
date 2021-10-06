using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Intimacies;
using Application.Intimacies.DTOs;

namespace API.Controllers
{
	[Authorize]
	public class IntimaciesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetIntimacies(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListIntimacies.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{userOneId}")]
		public async Task<IActionResult> GetIntimacy(int userOneId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailIntimacy.Query { UserOneId = userOneId }, ct));
		}


		[HttpPost]
		public async Task<IActionResult> CreateIntimacies(
			IntimacyCreateEditDTO intimacyCreateEditDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new CreateIntimacy.Command { IntimacyCreateEditDTO = intimacyCreateEditDTO }, ct));
		}

		[HttpPut]
		public async Task<IActionResult> EditIntimacies(IntimacyCreateEditDTO intimacyCreateEditDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new EditIntimacy.Command { IntimacyCreateEditDTO = intimacyCreateEditDTO }, ct));
		}
	}
}