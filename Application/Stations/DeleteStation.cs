using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using Persistence;
using Application.Core;

namespace Application.Stations
{
	public class DeleteStation
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int StationId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteStation> _logger;
			public Handler(DataContext context, ILogger<DeleteStation> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var station = await _context.Station
						.FindAsync(new object[] { request.StationId }, cancellationToken);
					if (station == null) return null!;

					station.IsDeleted = !station.IsDeleted;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to delete station by stationId: " + request.StationId);
						return Result<Unit>.Failure("Failed to delete station by stationId: " + request.StationId);
					}
					else
					{
						_logger.LogInformation("Successfully deleted station by stationId: " + request.StationId);
						return Result<Unit>
							.Success(Unit.Value, "Successfully deleted station by stationId: " + request.StationId);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}