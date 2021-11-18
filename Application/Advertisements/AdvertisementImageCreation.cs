using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AdvertisementImageCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int advertisementId, List<string> advertisementImages)
            {
                AdvertisementId = advertisementId;
                AdvertisementImages = advertisementImages;
            }

            public List<string> AdvertisementImages { get; }
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
                var advertisement =
                    await _context.Advertisement.FindAsync(new object[] {request.AdvertisementId}, cancellationToken);

                if (advertisement == null)
                {
                    _logger.LogInformation("Advertisement doesn't exist");
                    return Result<Unit>.NotFound("Advertisement doesn't exist.");
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.AdvertisementImages.Count == 0)
                    {
                        _logger.LogInformation("Advertisement image list must be provided");
                        return Result<Unit>.Failure("Advertisement image list must be provided.");
                    }

                    List<AdvertisementImage> advertisementImages = request.AdvertisementImages
                        .Select(advertisementImage => new AdvertisementImage
                        {
                            AdvertisementId = advertisement.AdvertisementId,
                            AdvertisementImageUrl = advertisementImage
                        })
                        .ToList();

                    await _context.AdvertisementImage.AddRangeAsync(advertisementImages, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new advertisement image");
                        return Result<Unit>.Failure("Failed to create new advertisement image.");
                    }

                    _logger.LogInformation("Successfully created new advertisement image");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new advertisement image.",
                        advertisementImages.First().AdvertisementId.ToString());
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