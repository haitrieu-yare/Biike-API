using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
	public class ListRoute
	{
		public class Query : IRequest<Result<List<RouteDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<RouteDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListRoute> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListRoute> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<RouteDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var routes = await _context.Route
						.Where(r => r.IsDeleted != true)
						.ProjectTo<RouteDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all route");
					return Result<List<RouteDTO>>.Success(routes);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RouteDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}