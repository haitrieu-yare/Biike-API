using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Bikes
{
	public class DeleteBike
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteBike> _logger;
			public Handler(DataContext context, ILogger<DeleteBike> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bike = await _context.Bike
						.Where(b => b.UserId == request.UserId)
						.SingleOrDefaultAsync(cancellationToken);

					if (bike == null) return null!;

					_context.Bike.Remove(bike);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to delete bike by userId {request.UserId}.");
						return Result<Unit>.Failure($"Failed to delete bike by userId {request.UserId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully deleted bike by userId {request.UserId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully deleted bike by userId {request.UserId}.");
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