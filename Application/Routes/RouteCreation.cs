using System;
using System.Linq;
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
    public class RouteCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public RouteCreationDto RouteCreationDto { get; init; } = null!;
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

                    Task<Route> oldRoute = _context.Route
                        .Where(r => r.DepartureId == request.RouteCreationDto.DepartureId)
                        .Where(r => r.DestinationId == request.RouteCreationDto.DestinationId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (oldRoute != null)
                    {
                        _logger.LogInformation(
                            "Route with departureId {DepartureId} and destinationId {DestinationId} is already existed",
                            request.RouteCreationDto.DepartureId, request.RouteCreationDto.DestinationId);
                        return Result<Unit>.Failure(
                            $"Route with departureId {request.RouteCreationDto.DepartureId} and destinationId {request.RouteCreationDto.DestinationId} is already existed.");
                    }

                    var newRoute = new Route();
                    _mapper.Map(request.RouteCreationDto, newRoute);
                    await _context.Route.AddAsync(newRoute, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new route");
                        return Result<Unit>.Failure("Failed to create new route.");
                    }

                    _logger.LogInformation("Successfully created route");
                    return Result<Unit>.Success(Unit.Value, "Successfully created route.", newRoute.RouteId.ToString());
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