using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Stations
{
	public class ListStations
	{
		public class Query : IRequest<Result<List<StationDTO>>>
		{
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<StationDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListStations> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListStations> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<StationDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					List<StationDTO> stations = new List<StationDTO>();

					if (request.IsAdmin)
					{
						stations = await _context.Station
							.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}
					else
					{
						stations = await _context.Station
							.Where(s => s.IsDeleted != true)
							.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
						// Set to null to make unnecessary fields excluded from response body.
						stations.ForEach(s =>
						{
							s.CreatedDate = null;
							s.IsDeleted = null;
						});
					}

					_logger.LogInformation("Successfully retrieved list of all stations.");
					return Result<List<StationDTO>>.Success(stations, "Successfully retrieved list of all stations.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<StationDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}