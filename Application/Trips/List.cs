using System.Collections.Generic;
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
	public class List
	{
		public class Query : IRequest<Result<List<TripDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<TripDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<List> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<List> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<TripDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trips = await _context.Trip
						.ProjectTo<TripDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all trip");
					return Result<List<TripDTO>>.Success(trips);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}