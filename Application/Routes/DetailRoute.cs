using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Routes
{
	public class DetailRoute
	{
		public class Query : IRequest<Result<RouteDTO>>
		{
			public int RouteId { get; set; }
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<RouteDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailRoute> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailRoute> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<RouteDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					RouteDTO route = new RouteDTO();

					if (request.IsAdmin)
					{
						Route routeDB = await _context.Route
							.FindAsync(new object[] { request.RouteId }, cancellationToken);

						_mapper.Map(routeDB, route);
					}
					else
					{
						route = await _context.Route
							.Where(r => r.RouteId == request.RouteId)
							.Where(r => r.IsDeleted != true)
							.ProjectTo<RouteDTO>(_mapper.ConfigurationProvider)
							.FirstOrDefaultAsync(cancellationToken);
						// Set to null to make unnecessary fields excluded from response body.
						route.CreatedDate = null;
						route.IsDeleted = null;
					}

					_logger.LogInformation($"Successfully retrieved route by routeId {request.RouteId}.");
					return Result<RouteDTO>.Success(
						route, $"Successfully retrieved route by routeId {request.RouteId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<RouteDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}