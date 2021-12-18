using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Sos
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SosDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int sosId, int userId)
            {
                SosId = sosId;
                UserId = userId;
            }

            public int SosId { get; }
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

                    Domain.Entities.Sos? sos =
                        await _context.Sos.FindAsync(new object[] {request.SosId}, cancellationToken);

                    if (sos == null)
                    {
                        _logger.LogInformation("Sos doesn't exist");
                        return Result<Unit>.NotFound("Sos doesn't exist.");
                    }
                    
                    if (sos.UserId != request.UserId)
                    {
                        _logger.LogInformation("This sos doesn't belong to this user");
                        return Result<Unit>.Failure("This sos doesn't belong to this user.");
                    }

                    _context.Sos.Remove(sos);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete sos by sosId {request.SosId}", request.SosId);
                        return Result<Unit>.Failure($"Failed to delete sos by sosId {request.SosId}.");
                    }

                    _logger.LogInformation("Successfully deleted sos by sosId {request.SosId}", request.SosId);
                    return Result<Unit>.Success(Unit.Value, $"Successfully deleted sos by sosId {request.SosId}.");
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