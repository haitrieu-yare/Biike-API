using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using Persistence;
using Application.Core;

namespace Application.Routes
{
	public class DeleteRoute
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int RouteId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteRoute> _logger;
			public Handler(DataContext context, ILogger<DeleteRoute> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var route = await _context.Route
						.FindAsync(new object[] { request.RouteId }, cancellationToken);

					if (route == null) return null!;

					route.IsDeleted = !route.IsDeleted;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to delete route by routeId {request.RouteId}.");
						return Result<Unit>.Failure($"Failed to delete route by routeId {request.RouteId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully deleted route by routeId {request.RouteId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully deleted route by routeId {request.RouteId}.");
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