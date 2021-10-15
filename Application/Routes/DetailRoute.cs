using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Routes.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
    public class DetailRoute
    {
        public class Query : IRequest<Result<RouteDto>>
        {
            public int RouteId { get; init; }
            public bool IsAdmin { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<RouteDto>>
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

            public async Task<Result<RouteDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    RouteDto route = new();

                    if (request.IsAdmin)
                    {
                        Route routeDb = await _context.Route
                            .FindAsync(new object[] {request.RouteId}, cancellationToken);

                        if (routeDb == null)
                        {
                            _logger.LogInformation("Route doesn't exist");
                            return Result<RouteDto>.NotFound("Route doesn't exist.");
                        }

                        _mapper.Map(routeDb, route);
                    }
                    else
                    {
                        route = await _context.Route
                            .Where(r => r.RouteId == request.RouteId)
                            .Where(r => r.IsDeleted != true)
                            .ProjectTo<RouteDto>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync(cancellationToken);

                        if (route == null)
                        {
                            _logger.LogInformation("Route doesn't exist");
                            return Result<RouteDto>.NotFound("Route doesn't exist.");
                        }

                        // Set to null to make unnecessary fields excluded from the response body.
                        route.CreatedDate = null;
                        route.IsDeleted = null;
                    }

                    _logger.LogInformation("Successfully retrieved route by routeId {request.RouteId}",
                        request.RouteId);
                    return Result<RouteDto>.Success(
                        route, $"Successfully retrieved route by routeId {request.RouteId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<RouteDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}