using System.Collections.Generic;
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

namespace Application.Stations
{
	public class List
	{
		public class Query : IRequest<Result<List<StationDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<StationDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<List> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<List> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<StationDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var stations = await _context.Station
						.Where(s => s.IsDeleted != true)
						.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all station");
					return Result<List<StationDTO>>.Success(stations);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<StationDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}