using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Advertisements.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int advertisementId, AdvertisementEditDto newAdvertisement)
            {
                AdvertisementId = advertisementId;
                NewAdvertisement = newAdvertisement;
            }

            public int AdvertisementId { get; }
            public AdvertisementEditDto NewAdvertisement { get; }
        }

        // ReSharper disable once UnusedType.Global
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

                    Advertisement? oldAdvertisement =
                        await _context.Advertisement.FindAsync(new object[] {request.AdvertisementId},
                            cancellationToken);

                    if (oldAdvertisement == null)
                    {
                        _logger.LogInformation("Advertisement doesn't exist");
                        return Result<Unit>.NotFound("Advertisement doesn't exist.");
                    }

                    _mapper.Map(request.NewAdvertisement, oldAdvertisement);

                    if (oldAdvertisement.EndDate.CompareTo(oldAdvertisement.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }

                    if (request.NewAdvertisement.AddressIds != null)
                    {
                        List<int> oldAddressIds = await _context.AdvertisementAddress
                            .Where(v => v.AdvertisementId == request.AdvertisementId)
                            .Select(v => v.AddressId)
                            .ToListAsync(cancellationToken);

                        List<AdvertisementAddress> advertisementAddresses = new();
                        foreach (var addressId in request.NewAdvertisement.AddressIds)
                        {
                            if (!oldAddressIds.Contains(addressId))
                            {
                                advertisementAddresses.Add(new AdvertisementAddress
                                {
                                    AddressId = addressId, AdvertisementId = request.AdvertisementId
                                });
                            }
                        }

                        await _context.AdvertisementAddress.AddRangeAsync(advertisementAddresses, cancellationToken);

                        foreach (var oldAddressId in oldAddressIds)
                        {
                            if (request.NewAdvertisement.AddressIds.Contains(oldAddressId)) continue;

                            var advertisementAddress = await _context.AdvertisementAddress
                                .Where(v => v.AdvertisementId == request.AdvertisementId)
                                .Where(v => v.AddressId == oldAddressId)
                                .SingleOrDefaultAsync(cancellationToken);

                            if (advertisementAddress != null)
                            {
                                _context.AdvertisementAddress.Remove(advertisementAddress);
                            }
                        }
                    }

                    if (request.NewAdvertisement.AdvertisementImages != null &&
                        request.NewAdvertisement.AdvertisementImages.Count > 0)
                    {
                        foreach (var advertisementImageDto in request.NewAdvertisement.AdvertisementImages)
                        {
                            AdvertisementImage? oldAdvertisementImage =
                                await _context.AdvertisementImage.FindAsync(
                                    new object[] {advertisementImageDto.AdvertisementImageId!}, cancellationToken);

                            if (oldAdvertisementImage == null)
                            {
                                _logger.LogInformation(
                                    "AdvertisementImage with AdvertisementImageId {AdvertisementImageId} doesn't exist",
                                    advertisementImageDto.AdvertisementImageId);
                                return Result<Unit>.NotFound(
                                    $"AdvertisementImage with AdvertisementImageId {advertisementImageDto.AdvertisementImageId} doesn't exist.");
                            }

                            oldAdvertisementImage.AdvertisementImageUrl = advertisementImageDto.AdvertisementImageUrl!;
                        }
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to update advertisement by advertisementId {request.AdvertisementId}",
                            request.AdvertisementId);
                        return Result<Unit>.Failure(
                            $"Failed to update advertisement by advertisementId {request.AdvertisementId}.");
                    }

                    _logger.LogInformation(
                        "Successfully updated advertisement by advertisementId {request.AdvertisementId}",
                        request.AdvertisementId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated advertisement by advertisementId {request.AdvertisementId}.");
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