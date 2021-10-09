using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Routes
{
	public class ListRoutes
	{
		public class Query : IRequest<Result<List<RouteDto>>>
		{
			public bool IsAdmin { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RouteDto>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<RouteDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<RouteDto>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Route.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<RouteDto> routes = new List<RouteDto>();

					if (request.Page <= lastPage)
					{
						if (request.IsAdmin)
						{
							routes = await _context.Route
								.OrderBy(r => r.RouteId)
								.Skip((request.Page - 1) * request.Limit)
								.Take(request.Limit)
								.ProjectTo<RouteDto>(_mapper.ConfigurationProvider)
								.ToListAsync(cancellationToken);
						}
						else
						{
							routes = await _context.Route
								.Where(r => r.IsDeleted != true)
								.OrderBy(r => r.RouteId)
								.Skip((request.Page - 1) * request.Limit)
								.Take(request.Limit)
								.ProjectTo<RouteDto>(_mapper.ConfigurationProvider)
								.ToListAsync(cancellationToken);
							// Set to null to make unnecessary fields excluded from response body.
							routes.ForEach(r =>
							{
								r.CreatedDate = null;
								r.IsDeleted = null;
							});
						}
					}

					PaginationDto paginationDto = new PaginationDto(
						request.Page, request.Limit, routes.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all routes");
					return Result<List<RouteDto>>.Success(
						routes, "Successfully retrieved list of all routes.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RouteDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}