using System.Linq;
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
	public class DetailBikeByUserId
	{
		public class Query : IRequest<Result<BikeDTO>>
		{
			public int UserId { get; set; }
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<BikeDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<BikeDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bike = await _context.Bike
						.Where(b => b.UserId == request.UserId)
						.ProjectTo<BikeDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					if (bike == null)
					{
						_logger.LogInformation($"Cound not found bike with UserId {request.UserId}");
						return Result<BikeDTO>.NotFound($"Cound not found bike with UserId {request.UserId}.");
					}

					if (!request.IsAdmin)
					{
						// Set to null to make unnecessary fields excluded from response body.
						bike.CreatedDate = null;
					}

					_logger.LogInformation($"Successfully retrieved bike by UserId {request.UserId}");
					return Result<BikeDTO>.Success(bike, $"Successfully retrieved bike by UserId {request.UserId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<BikeDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}