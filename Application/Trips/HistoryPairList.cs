using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Trips
{
	public class HistoryPairList
	{
		public class Query : IRequest<Result<List<TripPairDTO>>>
		{
			public int UserOneId { get; set; }
			public int UserTwoId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripPairDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<HistoryPairList> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<HistoryPairList> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<TripPairDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var pairTripsAsKeer = await _context.Trip
						.Where(t => t.KeerId == request.UserOneId)
						.Where(t => t.BikerId == request.UserTwoId)
						.Where(t => t.Status == (int)TripStatus.Finished
							|| t.Status == (int)TripStatus.Cancelled)
						.ProjectTo<TripPairDTO>(_mapper.ConfigurationProvider,
							new { userTwoId = request.UserTwoId })
						.ToListAsync(cancellationToken);

					var pairTripsAsBiker = await _context.Trip
						.Where(t => t.KeerId == request.UserTwoId)
						.Where(t => t.BikerId == request.UserOneId)
						.Where(t => t.Status == (int)TripStatus.Finished
							|| t.Status == (int)TripStatus.Cancelled)
						.ProjectTo<TripPairDTO>(_mapper.ConfigurationProvider,
							new { userTwoId = request.UserTwoId })
						.ToListAsync(cancellationToken);

					List<TripPairDTO> trips = pairTripsAsKeer.Concat(pairTripsAsBiker).ToList();

					_logger.LogInformation("Successfully retrieved list of all history pair trip.");
					return Result<List<TripPairDTO>>.Success(
						trips, "Successfully retrieved list of all history pair trip.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<TripPairDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}