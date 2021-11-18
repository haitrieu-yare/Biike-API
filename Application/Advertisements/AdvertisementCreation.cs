using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Advertisements.DTOs;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int creatorId, AdvertisementCreationDto advertisementCreationDto)
            {
                AdvertisementCreationDto = advertisementCreationDto;
                CreatorId = creatorId;
            }

            public int CreatorId { get; set; }
            public AdvertisementCreationDto AdvertisementCreationDto { get; }
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
                await using IDbContextTransaction transaction =
                    await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Advertisement advertisement = new();

                    _mapper.Map(request.AdvertisementCreationDto, advertisement);
                    advertisement.CreatorId = request.CreatorId;

                    if (advertisement.EndDate.CompareTo(advertisement.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }

                    await _context.Advertisement.AddAsync(advertisement, cancellationToken);
                    var advertisementResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!advertisementResult)
                    {
                        _logger.LogInformation("Failed to create new advertisement");
                        return Result<Unit>.Failure("Failed to create new advertisement.");
                    }

                    if (request.AdvertisementCreationDto.AddressIds!.Count > 0)
                    {
                        List<AdvertisementAddress> advertisementAddresses = request.AdvertisementCreationDto.AddressIds
                            .Select(addressId => new AdvertisementAddress
                            {
                                AddressId = addressId, AdvertisementId = advertisement.AdvertisementId
                            })
                            .ToList();

                        await _context.AdvertisementAddress.AddRangeAsync(advertisementAddresses, cancellationToken);
                        var advertisementAddressResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!advertisementAddressResult)
                        {
                            _logger.LogInformation("Failed to create new advertisement");
                            return Result<Unit>.Failure("Failed to create new advertisement.");
                        }
                    }

                    if (request.AdvertisementCreationDto.AdvertisementImages!.Count > 0)
                    {
                        List<AdvertisementImage> advertisementImages = request.AdvertisementCreationDto
                            .AdvertisementImages.Select(advertisementImage => new AdvertisementImage
                            {
                                AdvertisementId = advertisement.AdvertisementId,
                                AdvertisementImageUrl = advertisementImage
                            })
                            .ToList();

                        await _context.AdvertisementImage.AddRangeAsync(advertisementImages, cancellationToken);
                        var advertisementImageResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!advertisementImageResult)
                        {
                            _logger.LogInformation("Failed to create new advertisement");
                            return Result<Unit>.Failure("Failed to create new advertisement.");
                        }
                    }

                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Successfully created new advertisement");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new advertisement.",
                        advertisement.AdvertisementId.ToString());
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