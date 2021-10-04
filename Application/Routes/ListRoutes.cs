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
		public class Query : IRequest<Result<List<RouteDTO>>>
		{
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RouteDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListRoutes> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListRoutes> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<RouteDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					List<RouteDTO> routes = new List<RouteDTO>();

					if (request.IsAdmin)
					{
						routes = await _context.Route
							.ProjectTo<RouteDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}
					else
					{
						routes = await _context.Route
							.Where(r => r.IsDeleted != true)
							.ProjectTo<RouteDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
						// Set to null to make unnecessary fields excluded from response body.
						routes.ForEach(r =>
						{
							r.CreatedDate = null;
							r.IsDeleted = null;
						});
					}

					_logger.LogInformation("Successfully retrieved list of all routes.");
					return Result<List<RouteDTO>>.Success(routes, "Successfully retrieved list of all routes.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<RouteDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}