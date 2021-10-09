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
			string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
			string controllerName = Request.Path.ToString().Split("v1/").Last();

			if (result == null)
				return NotFound(NotFoundMessage);

			if (result.IsSuccess && result.Value == null)
				return NotFound(NotFoundMessage);

			if (!result.IsSuccess && !string.IsNullOrEmpty(result.NotFoundMessage))
			{
				return NotFound(result.NotFoundMessage);
			}

			if (result.IsSuccess && result.Value != null)
			{
				if (!string.IsNullOrEmpty(result.NewResourceId)) // CREATE - 201
				{
					string newResourceUrl = baseUrl + "/" + result.NewResourceId;

					return Created(newResourceUrl, new
					{
						message = result.SuccessMessage,
						data = result.Value,
					});
				}
				else if (result.PaginationDto != null)  // PAGINATION
				{
					#region Reconstruct a query string
					List<string> queryString = Request.QueryString.ToString().Split("&").ToList();

					string firstElement = queryString[0].Remove(0, 1);
					queryString[0] = firstElement;

					List<string> newQueryString = new List<string>();

					for (int i = 0; i < queryString.Count; i++)
					{
						if (!queryString[i].Contains("page") && !queryString[i].Contains("limit"))
						{
							newQueryString.Add(queryString[i]);
						}
					}

					newQueryString.Add("");

					string completeQueryString = string.Join("&", newQueryString.ToArray());
					#endregion

					List<object> link = new List<object>
					{
						new
						{
							href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page}" +
								$"&limit={result.PaginationDto.Limit}",
							rel = "self",
						},
						new {
							href = $"/{controllerName}?{completeQueryString}page=1&limit={result.PaginationDto.Limit}",
							rel = "first",
						},
						new {
							href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.LastPage}" +
								$"&limit={result.PaginationDto.Limit}",
							rel = "last",
						}
					};

					if (result.PaginationDto.Page > 1 && result.PaginationDto.Page <= result.PaginationDto.LastPage)
					{
						link.Add(new
						{
							href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page - 1}" +
								$"&limit={result.PaginationDto.Limit}",
							rel = "prev",
						});
					}

					if (result.PaginationDto.Page >= 1 && result.PaginationDto.Page < result.PaginationDto.LastPage)
					{
						link.Add(new
						{
							href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page + 1}" +
								$"&limit={result.PaginationDto.Limit}",
							rel = "next",
						});
					}

					var response = new
					{
						message = result.SuccessMessage,
						_meta = new
						{
							page = result.PaginationDto.Page,
							limit = result.PaginationDto.Limit,
							count = result.PaginationDto.Count,
							totalRecord = result.PaginationDto.TotalRecord,
						},
						_link = link,
						data = result.Value,
					};

					return Ok(response);
				}
				else // NORMAL - 200
				{
					return Ok(new
					{
						message = result.SuccessMessage,
						data = result.Value,
					});
				}
			}

			if (!result.IsSuccess && result.IsUnauthorized) // FORBIDDEN
			{
				return Forbid();
			}

			return BadRequest(result.ErrorMessage);
		}
	}
}