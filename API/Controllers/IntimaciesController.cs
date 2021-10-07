using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Intimacies;
using Application.Intimacies.DTOs;
using Domain.Enums;

namespace API.Controllers
{
	[Authorize]
	public class IntimaciesController : BaseApiController
	{
		[Authorized(RoleStatus.Admin)]
		[HttpGet]
		public async Task<IActionResult> GetIntimacies(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListIntimacies.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{userOneId:int}")]
		public async Task<IActionResult> GetIntimaciesByUserId(int userOneId, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, userOneId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of intimacy.");

			return HandleResult(await Mediator.Send(new ListIntimaciesByUserId.Query { UserOneId = userOneId }, ct));
		}

		[HttpPost]
		public async Task<IActionResult> CreateIntimacies(
			IntimacyCreateEditDTO intimacyCreateEditDTO, CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, intimacyCreateEditDTO.UserOneId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of intimacy.");

			return HandleResult(await Mediator.Send(
				new CreateIntimacy.Command { IntimacyCreateEditDTO = intimacyCreateEditDTO }, ct));
		}

		[HttpPut]
		public async Task<IActionResult> EditIntimacies(IntimacyCreateEditDTO intimacyCreateEditDTO,
			CancellationToken ct)
		{
			ValidationDTO validationDto = ControllerUtils.Validate(HttpContext, intimacyCreateEditDTO.UserOneId);

			if (!validationDto.IsUserFound)
				return BadRequest("Can't get userId who send the request.");

			if (!validationDto.IsAuthorized)
				return BadRequest("UserId of requester isn't the same with userId of intimacy.");

			return HandleResult(await Mediator.Send(
				new EditIntimacy.Command { IntimacyCreateEditDTO = intimacyCreateEditDTO }, ct));
		}
	}
}