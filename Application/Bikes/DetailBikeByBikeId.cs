using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Bikes
{
	public class DetailBikeByBikeId
	{
		public class Query : IRequest<Result<BikeDto>>
		{
			public int BikeId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<BikeDto>>
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

			public async Task<Result<BikeDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bikeDb = await _context.Bike
						.FindAsync(new object[] { request.BikeId }, cancellationToken);

					if (bikeDb == null)
					{
						_logger.LogInformation($"Cound not found bike with BikeId {request.BikeId}");
						return Result<BikeDto>.NotFound($"Cound not found bike with BikeId {request.BikeId}.");
					}

					BikeDto bike = new BikeDto();

					_mapper.Map(bikeDb, bike);

					_logger.LogInformation($"Successfully retrieved bike by BikeId {request.BikeId}");
					return Result<BikeDto>.Success(bike, $"Successfully retrieved bike by BikeId {request.BikeId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<BikeDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}