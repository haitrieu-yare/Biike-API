using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Advertisements.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementImageDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(AdvertisementImageDeletionDto advertisementImageDeletionDto)
            {
                AdvertisementImageDeletionDto = advertisementImageDeletionDto;
            }

            public AdvertisementImageDeletionDto AdvertisementImageDeletionDto { get; }
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

                    Advertisement? advertisement = await _context.Advertisement.FindAsync(
                        new object[] {request.AdvertisementImageDeletionDto.AdvertisementId!}, cancellationToken);

                    if (advertisement == null)
                    {
                        _logger.LogInformation("Advertisement doesn't exist");
                        return Result<Unit>.NotFound("Advertisement doesn't exist.");
                    }

                    if (request.AdvertisementImageDeletionDto.AdvertisementImageIds!.Count == 0)
                    {
                        _logger.LogInformation("There are no imageId in request");
                        return Result<Unit>.NotFound("There are no imageId in request.");
                    }

                    foreach (var advertisementImageId in request.AdvertisementImageDeletionDto.AdvertisementImageIds)
                    {
                        var advertisementImage = await _context.AdvertisementImage
                            .Where(a => a.AdvertisementImageId == int.Parse(advertisementImageId))
                            .SingleOrDefaultAsync(cancellationToken);

                        if (advertisementImage == null) continue;

                        if (advertisementImage.AdvertisementId != request.AdvertisementImageDeletionDto.AdvertisementId)
                        {
                            _logger.LogInformation(
                                "AdvertisementImage with AdvertisementImageId {AdvertisementImageId} does not belong to advertisement " +
                                "with AdvertisementId {AdvertisementId}", advertisementImageId,
                                request.AdvertisementImageDeletionDto.AdvertisementId);
                            return Result<Unit>.NotFound(
                                $"AdvertisementImage with AdvertisementImageId {advertisementImageId} does not belong to advertisement " +
                                $"with AdvertisementId {request.AdvertisementImageDeletionDto.AdvertisementId}.");
                        }

                        _context.AdvertisementImage.Remove(advertisementImage);
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to delete advertisement image by advertisementId {request.AdvertisementId}",
                            request.AdvertisementImageDeletionDto.AdvertisementId);
                        return Result<Unit>.Failure(
                            $"Failed to delete advertisement image by advertisementId {request.AdvertisementImageDeletionDto.AdvertisementId}.");
                    }

                    _logger.LogInformation(
                        "Successfully deleted advertisement image by advertisementId {request.AdvertisementId}",
                        request.AdvertisementImageDeletionDto.AdvertisementId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted advertisement image by advertisementId {request.AdvertisementImageDeletionDto.AdvertisementId}.");
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