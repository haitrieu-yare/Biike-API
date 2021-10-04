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
		private const string NotFoundMessage = "No records found.";
		// private string baseURL = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";

		protected ActionResult HandleResult<T>(Result<T> result)
		{
			string baseURL = $"{Request.Scheme}://{Request.Host}{Request.Path}";

			if (result == null)
				return NotFound(NotFoundMessage);

			if (result.IsSuccess && result.Value == null)
				return NotFound(NotFoundMessage);

			if (result.IsSuccess && result.Value != null)
				if (!string.IsNullOrEmpty(result.NewResourceId))
				{
					string newResourceURL = baseURL + "/" + result.NewResourceId;

					return Created(newResourceURL, new
					{
						message = result.SuccessMessage,
						data = result.Value,
					});
				}
				else
				{
					return Ok(new
					{
						message = result.SuccessMessage,
						data = result.Value,
					});
				}

			return BadRequest(result.ErrorMessage);
		}
	}
}