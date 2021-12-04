using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.BikeAvailabilities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeAvailabilityDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int bikeAvailabilityId, int userId)
            {
                BikeAvailabilityId = bikeAvailabilityId;
                UserId = userId;
            }

            public int BikeAvailabilityId { get; }
            public int UserId { get; }
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

                    var bikeAvailability = await _context.BikeAvailability
                        .FindAsync(new object[] {request.BikeAvailabilityId}, cancellationToken);
                    
                    if (bikeAvailability == null)
                    {
                        _logger.LogInformation("Bike availability doesn't exist");
                        return Result<Unit>.NotFound("Bike availability doesn't exist.");
                    }

                    if (bikeAvailability.UserId != request.UserId)
                    {
                        _logger.LogInformation("This bike availability doesn't belong to user with UserId {UserId}",
                            request.UserId);
                        return Result<Unit>.Failure(
                            $"This bike availability doesn't belong to user with UserId {request.UserId}.");
                    }

                    _context.BikeAvailability.Remove(bikeAvailability);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete bike availability");
                        return Result<Unit>.Failure("Failed to delete bike availability.");
                    }

                    _logger.LogInformation("Successfully deleted bike availability");
                    return Result<Unit>.Success(Unit.Value, "Successfully deleted bike availability.");
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