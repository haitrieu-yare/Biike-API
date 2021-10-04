using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Stations.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Stations
{
	public class EditStation
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int StationId { get; set; }
			public StationDTO NewStationDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditStation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditStation> logger)
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

					var oldStation = await _context.Station
						.FindAsync(new object[] { request.StationId }, cancellationToken);

					if (oldStation == null) return null!;

					if (oldStation.IsDeleted)
					{
						_logger.LogInformation($"Station with StationId {request.StationId} has been deleted. " +
							"Please reactivate it if you want to edit it.");
						return Result<Unit>.Failure($"Station with StationId {request.StationId} has been deleted. " +
							"Please reactivate it if you want to edit it.");
					}

					_mapper.Map(request.NewStationDTO, oldStation);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to update station by stationId {request.StationId}.");
						return Result<Unit>.Failure($"Failed to update station by stationId {request.StationId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully updated station by stationId {request.StationId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully updated station by stationId {request.StationId}.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}