using System.Threading;
using System.Threading.Tasks;
using Application.Intimacies;
using Application.Intimacies.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class IntimaciesController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetIntimacies(CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{userOneId}")]
		public async Task<IActionResult> GetIntimacy(int userOneId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { UserOneId = userOneId }, ct));
		}


		[HttpPost]
		public async Task<IActionResult> CreateIntimacies(IntimacyCreateDTO intimacyCreateDTO, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new Create.Command { IntimacyCreateDTO = intimacyCreateDTO }, ct));
		}

		[HttpPut]
		public async Task<IActionResult> EditIntimacies(IntimacyEditDTO intimacyEditDTO,
			CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Edit.Command
			{ IntimacyEditDTO = intimacyEditDTO }, ct));
		}
	}
}