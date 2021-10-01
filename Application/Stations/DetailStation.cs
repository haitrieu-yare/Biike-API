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
using System.Collections.Generic;

namespace Application.Stations
{
	public class DetailStation
	{
		public class Query : IRequest<Result<StationDTO>>
		{
			public int StationId { get; set; }
			public bool IsAdmin { get; set; }
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

					StationDTO station = new StationDTO();
					if (request.IsAdmin)
					{
						station = await _context.Station
							.Where(s => s.StationId == request.StationId)
							.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
							.SingleOrDefaultAsync(cancellationToken);
					}
					else
					{
						station = await _context.Station
							.Where(s => s.StationId == request.StationId)
							.Where(s => s.IsDeleted != true)
							.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
							.SingleOrDefaultAsync(cancellationToken);
						// Set to null to make this field excluded from response body.
						station.IsDeleted = null;
					}

					_logger.LogInformation("Successfully retrieved station by stationId: " + request.StationId);
					return Result<StationDTO>
						.Success(station, "Successfully retrieved station by stationId: " + request.StationId);
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