using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementClickCountEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int advertisementId)
            {
                AdvertisementId = advertisementId;
            }

            public int AdvertisementId { get; }
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

                    Advertisement? advertisement =
                        await _context.Advertisement.FindAsync(new object[] {request.AdvertisementId},
                            cancellationToken);

                    if (advertisement == null)
                    {
                        _logger.LogInformation("Advertisement doesn't exist");
                        return Result<Unit>.NotFound("Advertisement doesn't exist.");
                    }

                    advertisement.TotalClickCount++;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to update click count of advertisement by advertisementId {request.AdvertisementId}",
                            request.AdvertisementId);
                        return Result<Unit>.Failure(
                            $"Failed to update click count of advertisement by advertisementId {request.AdvertisementId}.");
                    }

                    _logger.LogInformation(
                        "Successfully updated click count of advertisement by advertisementId {request.AdvertisementId}",
                        request.AdvertisementId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated click count of advertisement by advertisementId {request.AdvertisementId}.");
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