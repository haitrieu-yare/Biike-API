using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Bikes
{
	public class ListBikes
	{
		public class Query : IRequest<Result<List<BikeDTO>>>
		{
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<BikeDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListBikes> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListBikes> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<BikeDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bikes = await _context.Bike
						.ProjectTo<BikeDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					if (!request.IsAdmin)
					{
						// Set to null to make unnecessary fields excluded from response body.
						bikes.ForEach(b => b.CreatedDate = null);
					}

					_logger.LogInformation("Successfully retrieved list of all bikes.");
					return Result<List<BikeDTO>>.Success(bikes, "Successfully retrieved list of all bikes.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<BikeDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}