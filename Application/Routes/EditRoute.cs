using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Routes
{
	public class EditRoute
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int RouteId { get; set; }
			public RouteDto RouteDto { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditRoute> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditRoute> logger)
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

					var oldRoute = await _context.Route
						.FindAsync(new object[] { request.RouteId }, cancellationToken);

					if (oldRoute == null) return null!;

					if (oldRoute.IsDeleted)
					{
						_logger.LogInformation($"Station with RouteId {request.RouteId} has been deleted. " +
							"Please reactivate it if you want to edit it.");
						return Result<Unit>.Failure($"Station with RouteId {request.RouteId} has been deleted. " +
							"Please reactivate it if you want to edit it.");
					}

					_mapper.Map(request.RouteDto, oldRoute);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to update route by routeId {request.RouteId}.");
						return Result<Unit>.Failure($"Failed to update route by routeId {request.RouteId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully updated route by routeId {request.RouteId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully updated route by routeId {request.RouteId}.");
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