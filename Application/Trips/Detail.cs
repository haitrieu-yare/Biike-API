using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class Detail
	{
		public class Query : IRequest<Result<TripDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Detail> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Detail> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<TripDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trip = await _context.Trip
						.ProjectTo<TripDTO>(_mapper.ConfigurationProvider)
						.FirstOrDefaultAsync(t => t.TripId == request.Id, cancellationToken);

					_logger.LogInformation("Successfully retrieved trip");
					return Result<TripDTO>.Success(trip);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}