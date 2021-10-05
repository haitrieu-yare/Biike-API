using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Application.Core;
using MediatR;

namespace API.Controllers
{
	[ApiController]
	[Route("api/biike/v1/[controller]")]
	public class BaseApiController : ControllerBase
	{
		private IMediator? _mediator;
		protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
		private const string NotFoundMessage = "No records found.";
		protected ActionResult HandleResult<T>(Result<T> result)
		{
			string baseURL = $"{Request.Scheme}://{Request.Host}{Request.Path}";
			string controllerName = Request.Path.ToString().Split("/").Last();

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
				else if (result.PaginationDTO != null)
				{
					List<object> link = new List<object>
					{
						new
						{
							href = $"/{controllerName}?page={result.PaginationDTO.Page}" +
								$"&limit={result.PaginationDTO.Limit}",
							rel = "self",
						},
						new {
							href = $"/{controllerName}?page=1&limit={result.PaginationDTO.Limit}",
							rel = "first",
						},
						new {
							href = $"/{controllerName}?page={result.PaginationDTO.LastPage}" +
								$"&limit={result.PaginationDTO.Limit}",
							rel = "last",
						}
					};

					if (result.PaginationDTO.Page > 1 && result.PaginationDTO.Page <= result.PaginationDTO.LastPage)
					{
						link.Add(new
						{
							href = $"/{controllerName}?page={result.PaginationDTO.Page - 1}" +
								$"&limit={result.PaginationDTO.Limit}",
							rel = "prev",
						});
					}

					if (result.PaginationDTO.Page >= 1 && result.PaginationDTO.Page < result.PaginationDTO.LastPage)
					{
						link.Add(new
						{
							href = $"/{controllerName}?page={result.PaginationDTO.Page + 1}" +
								$"&limit={result.PaginationDTO.Limit}",
							rel = "next",
						});
					}

					var response = new
					{
						message = result.SuccessMessage,
						_meta = new
						{
							page = result.PaginationDTO.Page,
							limit = result.PaginationDTO.Limit,
							count = result.PaginationDTO.Count,
							totalRecord = result.PaginationDTO.TotalRecord,
						},
						_link = link,
						data = result.Value,
					};

					return Ok(response);
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