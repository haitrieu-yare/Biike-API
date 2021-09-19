using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
	public class Detail
	{
		public class Query : IRequest<Result<RouteDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<RouteDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Detail> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Detail> logger)
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

					var route = await _context.Route
						.Where(r => r.IsDeleted != true)
						.ProjectTo<RouteDTO>(_mapper.ConfigurationProvider)
						.FirstOrDefaultAsync(r => r.Id == request.Id);

					_logger.LogInformation("Successfully retrieved route");
					return Result<RouteDTO>.Success(route);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<RouteDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}