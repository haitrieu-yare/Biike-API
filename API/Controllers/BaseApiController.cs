using System.Collections.Generic;
using System.Linq;
using Application;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/biike/v1/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private const string NotFoundMessage = "No records found.";
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            string controllerName = Request.Path.ToString().Split("v1/").Last();

            switch (result.IsSuccess)
            {
                case true when result.Value == null:
                    return NotFound(NotFoundMessage);
                case false when !string.IsNullOrEmpty(result.NotFoundMessage):
                    return NotFound(result.NotFoundMessage);
                case false when result.IsUnauthorized:
                    return new ObjectResult(result.UnauthorizedMessage) {StatusCode = 403};
                case true when result.Value != null:
                {
                    #region CREATED - 201

                    if (!string.IsNullOrEmpty(result.NewResourceId))
                    {
                        string baseUrl = $"{Request.Scheme}://{Request.Host}/api/biike/v1/{controllerName.Split("/").First()}";
                        string newResourceUrl = baseUrl + "/" + result.NewResourceId;
                        return Created(newResourceUrl, new {message = result.SuccessMessage, data = result.Value});
                    }

                    #endregion

                    #region NORMAL - 200

                    if (result.PaginationDto == null)
                        return Ok(new {message = result.SuccessMessage, data = result.Value});

                    #endregion

                    #region Pagination - 200

                    #region Reconstruct a query string

                    List<string> queryString = Request.QueryString.ToString().Split("&").ToList();

                    string firstElement = queryString[0].Remove(0, 1);
                    queryString[0] = firstElement;

                    List<string> newQueryString =
                        queryString.Where(t => !t.Contains("page") && !t.Contains("limit")).ToList();

                    newQueryString.Add("");

                    string completeQueryString = string.Join("&", newQueryString.ToArray());

                    #endregion

                    List<object> link = new()
                    {
                        new
                        {
                            href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page}" +
                                   $"&limit={result.PaginationDto.Limit}",
                            rel = "self"
                        },
                        new
                        {
                            href =
                                $"/{controllerName}?{completeQueryString}page=1&limit={result.PaginationDto.Limit}",
                            rel = "first"
                        },
                        new
                        {
                            href =
                                $"/{controllerName}?{completeQueryString}page={result.PaginationDto.LastPage}" +
                                $"&limit={result.PaginationDto.Limit}",
                            rel = "last"
                        }
                    };

                    if (result.PaginationDto.Page > 1 && result.PaginationDto.Page <= result.PaginationDto.LastPage)
                        link.Add(new
                        {
                            href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page - 1}" +
                                   $"&limit={result.PaginationDto.Limit}",
                            rel = "prev"
                        });

                    if (result.PaginationDto.Page >= 1 && result.PaginationDto.Page < result.PaginationDto.LastPage)
                        link.Add(new
                        {
                            href = $"/{controllerName}?{completeQueryString}page={result.PaginationDto.Page + 1}" +
                                   $"&limit={result.PaginationDto.Limit}",
                            rel = "next"
                        });

                    var response = new
                    {
                        message = result.SuccessMessage,
                        _meta = new
                        {
                            page = result.PaginationDto.Page,
                            limit = result.PaginationDto.Limit,
                            count = result.PaginationDto.Count,
                            totalRecord = result.PaginationDto.TotalRecord
                        },
                        _link = link,
                        data = result.Value
                    };

                    return Ok(response);

                    #endregion
                }
                default:
                    return BadRequest(result.ErrorMessage);
            }
        }
    }
}