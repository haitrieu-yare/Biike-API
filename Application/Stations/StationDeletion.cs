using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StationDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int StationId { get; init; }
        }

        // ReSharper disable once UnusedType.Global
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

                    Station station = await _context.Station
                        .FindAsync(new object[] {request.StationId}, cancellationToken);

                    if (station == null)
                    {
                        _logger.LogInformation("Station doesn't exist");
                        return Result<Unit>.NotFound("Station doesn't exist.");
                    }

                    station.IsDeleted = !station.IsDeleted;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete station by stationId {request.StationId}",
                            request.StationId);
                        return Result<Unit>.Failure($"Failed to delete station by stationId {request.StationId}.");
                    }

                    _logger.LogInformation("Successfully deleted station by stationId {request.StationId}",
                        request.StationId);
                    return Result<Unit>.Success(
                        Unit.Value, $"Successfully deleted station by stationId {request.StationId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}