using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class HistoryList
	{
		public class Query : IRequest<Result<List<TripHistoryDTO>>>
		{
			public int UserId { get; set; }
			public int Role { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripHistoryDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<HistoryList> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<HistoryList> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<TripHistoryDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripHistoryDTO = new List<TripHistoryDTO>();

					if (request.Role == (int)RoleStatus.Keer)
					{
						var tripHistoryKeerDTO = await _context.Trip
							.Where(t => t.KeerId == request.UserId)
							.Where(t => t.Status == (int)TripStatus.Finished
								|| t.Status == (int)TripStatus.Cancelled)
							.ProjectTo<KeerHistoryTripDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

						_mapper.Map(tripHistoryKeerDTO, tripHistoryDTO);
					}
					else if (request.Role == (int)RoleStatus.Biker)
					{
						var tripHistoryBikerDTO = await _context.Trip
							.Where(t => t.BikerId == request.UserId)
							.Where(t => t.Status == (int)TripStatus.Finished
								|| t.Status == (int)TripStatus.Cancelled)
							.ProjectTo<BikerHistoryTripDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

						_mapper.Map(tripHistoryBikerDTO, tripHistoryDTO);
					}

					_logger.LogInformation("Successfully retrieved list of all history trip");
					return Result<List<TripHistoryDTO>>.Success(tripHistoryDTO);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripHistoryDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}