using System.Linq;
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
	public class DetailTrip
	{
		public class Query : IRequest<Result<TripDetailDTO>>
		{
			public int TripId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripDetailDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailTrip> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailTrip> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<TripDetailDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trip = await _context.Trip
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripDetailDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation($"Successfully retrieved trip by TripId {request.TripId}.");
					return Result<TripDetailDTO>.Success(
						trip, $"Successfully retrieved trip by TripId {request.TripId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<TripDetailDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}