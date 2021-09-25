using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Persistence;
using Application.Core;
using Application.Stations.DTOs;

namespace Application.Stations
{
	public class DetailStation
	{
		public class Query : IRequest<Result<StationDTO>>
		{
			public int StationId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<StationDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailStation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailStation> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<StationDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var station = await _context.Station
						.Where(s => s.StationId == request.StationId)
						.Where(s => s.IsDeleted != true)
						.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved station by stationId: " + request.StationId);
					return Result<StationDTO>.Success(station);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<StationDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}