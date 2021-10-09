using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
	public class DetailBikeByBikeId
	{
		public class Query : IRequest<Result<BikeDto>>
		{
			public int BikeId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<BikeDto>>
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

			public async Task<Result<BikeDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bikeDb = await _context.Bike
						.FindAsync(new object[] { request.BikeId }, cancellationToken);

					if (bikeDb == null)
					{
						_logger.LogInformation("Could not found bike with BikeId {request.BikeId}", request.BikeId);
						return Result<BikeDto>.NotFound($"Could not found bike with BikeId {request.BikeId}.");
					}

					BikeDto bike = new();

					_mapper.Map(bikeDb, bike);

					_logger.LogInformation("Successfully retrieved bike by BikeId {request.BikeId}", request.BikeId);
					return Result<BikeDto>.Success(bike, $"Successfully retrieved bike by BikeId {request.BikeId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<BikeDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}