using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Stations
{
	public class DetailStation
	{
		public class Query : IRequest<Result<StationDto>>
		{
			public int StationId { get; set; }
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<StationDto>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailStation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailStation> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<StationDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					StationDto station = new StationDto();

					if (request.IsAdmin)
					{
						Station stationDb = await _context.Station
							.FindAsync(new object[] { request.StationId }, cancellationToken);

						_mapper.Map(stationDb, station);
					}
					else
					{
						station = await _context.Station
							.Where(s => s.StationId == request.StationId)
							.Where(s => s.IsDeleted != true)
							.ProjectTo<StationDto>(_mapper.ConfigurationProvider)
							.SingleOrDefaultAsync(cancellationToken);
						// Set to null to make unnecessary fields excluded from response body.
						station.CreatedDate = null;
						station.IsDeleted = null;
					}

					_logger.LogInformation($"Successfully retrieved station by stationId {request.StationId}.");
					return Result<StationDto>.Success(
						station, $"Successfully retrieved station by stationId {request.StationId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<StationDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}