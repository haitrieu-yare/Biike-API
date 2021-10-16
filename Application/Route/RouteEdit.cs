using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RouteEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int RouteId { get; init; }
            public RouteDto RouteDto { get; init; } = null!;
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

                    Route oldRoute =
                        await _context.Route.FindAsync(new object[] {request.RouteId}, cancellationToken);

                    if (oldRoute == null)
                    {
                        _logger.LogInformation("Route doesn't exist");
                        return Result<Unit>.NotFound("Route doesn't exist.");
                    }

                    if (oldRoute.IsDeleted)
                    {
                        _logger.LogInformation(
                            "Route with RouteId {request.RouteId} has been deleted. " +
                            "Please reactivate it if you want to edit it", request.RouteId);
                        return Result<Unit>.Failure($"Route with RouteId {request.RouteId} has been deleted. " +
                                                    "Please reactivate it if you want to edit it.");
                    }

                    _mapper.Map(request.RouteDto, oldRoute);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update route by routeId {request.RouteId}", request.RouteId);
                        return Result<Unit>.Failure($"Failed to update route by routeId {request.RouteId}.");
                    }

                    _logger.LogInformation("Successfully updated route by routeId {request.RouteId}", request.RouteId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated route by routeId {request.RouteId}.");
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