using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RouteDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int RouteId { get; init; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Route route = await _context.Route
                        .FindAsync(new object[] {request.RouteId}, cancellationToken);

                    if (route == null)
                    {
                        _logger.LogInformation("Route doesn't exist");
                        return Result<Unit>.NotFound("Route doesn't exist.");
                    }

                    route.IsDeleted = !route.IsDeleted;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete route by routeId {request.RouteId}", request.RouteId);
                        return Result<Unit>.Failure($"Failed to delete route by routeId {request.RouteId}.");
                    }

                    _logger.LogInformation("Successfully deleted route by routeId {request.RouteId}", request.RouteId);
                    return Result<Unit>.Success(
                        Unit.Value, $"Successfully deleted route by routeId {request.RouteId}.");
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