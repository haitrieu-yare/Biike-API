using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Application.Core;

namespace API.Controllers
{
	[ApiController]
	[Route("api/biike/v1/[controller]")]
	public class BaseApiController : ControllerBase
	{
		private IMediator? _mediator;
		protected IMediator Mediator => _mediator ??= HttpContext.RequestServices
			.GetService<IMediator>()!;

		private string NotFoundMessage = "No records found.";

		protected ActionResult HandleResult<T>(Result<T> result)
		{
			if (result == null)
				return NotFound(NotFoundMessage);

			if (result.IsSuccess && result.Value != null)
				return Ok(new
				{
					message = result.SuccessMessage,
					data = result.Value,
				});

			if (result.IsSuccess && result.Value == null)
				return NotFound(NotFoundMessage);

			return BadRequest(result.ErrorMessage);
		}
	}
}