using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
	public class DetailStation
	{
		public class Query : IRequest<Result<StationDto>>
		{
			public int StationId { get; init; }
			public bool IsAdmin { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<StationDto>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
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

					StationDto station = new();

					if (request.IsAdmin)
					{
						Station stationDb = await _context.Station
							.FindAsync(new object[] { request.StationId }, cancellationToken);
						
						if (stationDb == null)
						{
							_logger.LogInformation("Station doesn't exist");
							return Result<StationDto>.NotFound("Station doesn't exist.");
						}

						_mapper.Map(stationDb, station);
					}
					else
					{
						station = await _context.Station
							.Where(s => s.StationId == request.StationId)
							.Where(s => s.IsDeleted != true)
							.ProjectTo<StationDto>(_mapper.ConfigurationProvider)
							.SingleOrDefaultAsync(cancellationToken);
						
						if (station == null)
						{
							_logger.LogInformation("Station doesn't exist");
							return Result<StationDto>.NotFound("Station doesn't exist.");
						}
						
						// Set to null to make unnecessary fields excluded from the response body.
						station.CreatedDate = null;
						station.IsDeleted = null;
					}

					_logger.LogInformation("Successfully retrieved station by stationId {request.StationId}", request.StationId);
					return Result<StationDto>.Success(
						station, $"Successfully retrieved station by stationId {request.StationId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<StationDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}