using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class DetailTrip
	{
		public class Query : IRequest<Result<TripDetailDto>>
		{
			public int TripId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripDetailDto>>
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

			public async Task<Result<TripDetailDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trip = await _context.Trip
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripDetailDto>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation($"Successfully retrieved trip by TripId {request.TripId}");
					return Result<TripDetailDto>.Success(
						trip, $"Successfully retrieved trip by TripId {request.TripId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripDetailDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}