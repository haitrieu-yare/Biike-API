using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
	public class Delete
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Delete> _logger;
			public Handler(DataContext context, ILogger<Delete> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var station = await _context.Station.FindAsync(request.Id);
					if (station == null) return null;

					station.IsDeleted = true;
					var result = await _context.SaveChangesAsync() > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to delete station");
						return Result<Unit>.Failure("Failed to delete station");
					}
					else
					{
						_logger.LogInformation("Successfully deleted station");
						return Result<Unit>.Success(Unit.Value);
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