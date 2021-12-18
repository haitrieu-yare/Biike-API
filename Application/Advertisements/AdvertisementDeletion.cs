using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementDeletion
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

                    Advertisement? advertisement = await _context.Advertisement
                        .AsSingleQuery()
                        .Where(a => a.AdvertisementId == request.AdvertisementId)
                        .Include(a => a.AdvertisementAddresses)
                        .Include(a => a.AdvertisementImages)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (advertisement == null)
                    {
                        _logger.LogInformation("Advertisement doesn't exist");
                        return Result<Unit>.NotFound("Advertisement doesn't exist.");
                    }

                    _context.AdvertisementImage.RemoveRange(advertisement.AdvertisementImages);
                    _context.AdvertisementAddress.RemoveRange(advertisement.AdvertisementAddresses);
                    _context.Advertisement.Remove(advertisement);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to delete advertisement by advertisementId {request.AdvertisementId}",
                            request.AdvertisementId);
                        return Result<Unit>.Failure(
                            $"Failed to delete advertisement by advertisementId {request.AdvertisementId}.");
                    }

                    _logger.LogInformation(
                        "Successfully deleted advertisement by advertisementId {request.AdvertisementId}",
                        request.AdvertisementId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted advertisement by advertisementId {request.AdvertisementId}.");
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