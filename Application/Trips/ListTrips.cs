using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Trips
{
	public class ListTrips
	{
		public class Query : IRequest<Result<List<TripDetailDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<TripDetailDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListTrips> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListTrips> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<TripDetailDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trips = await _context.Trip
						.ProjectTo<TripDetailDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all trips");
					return Result<List<TripDetailDTO>>
						.Success(trips, "Successfully retrieved list of all trips");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDetailDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}