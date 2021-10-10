using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
	public class EditStation
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int StationId { get; init; }
			public StationDto NewStationDto { get; init; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
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

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Station oldStation = await _context.Station
						.FindAsync(new object[] { request.StationId }, cancellationToken);

					if (oldStation == null)
					{
						_logger.LogInformation("Station doesn't exist");
						return Result<Unit>.NotFound("Station doesn't exist.");
					}

					if (oldStation.IsDeleted)
					{
						_logger.LogInformation("Station with StationId {request.StationId} has been deleted. " +
						                       "Please reactivate it if you want to edit it", request.StationId);
						return Result<Unit>.Failure($"Station with StationId {request.StationId} has been deleted. " +
						                            "Please reactivate it if you want to edit it.");
					}

					_mapper.Map(request.NewStationDto, oldStation);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update station by stationId {request.StationId}",
							request.StationId);
						return Result<Unit>.Failure($"Failed to update station by stationId {request.StationId}.");
					}

					_logger.LogInformation("Successfully updated station by stationId {request.StationId}",
						request.StationId);
					return Result<Unit>.Success(
						Unit.Value, $"Successfully updated station by stationId {request.StationId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}