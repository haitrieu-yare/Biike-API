using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.BikeAvailabilities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.BikeAvailabilities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeAvailabilityDetails
    {
        public class Query : IRequest<Result<BikeAvailabilityDto>>
        {
            public Query(int bikeAvailabilityId, int userId)
            {
                BikeAvailabilityId = bikeAvailabilityId;
                UserId = userId;
            }

            public int BikeAvailabilityId { get; }
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<BikeAvailabilityDto>>
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

            public async Task<Result<BikeAvailabilityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bikeAvailability = await _context.BikeAvailability
                        .Where(b => b.BikeAvailabilityId == request.BikeAvailabilityId)
                        .ProjectTo<BikeAvailabilityDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bikeAvailability == null)
                    {
                        _logger.LogInformation("Bike availability doesn't exist");
                        return Result<BikeAvailabilityDto>.NotFound("Bike availability doesn't exist.");
                    }

                    if (bikeAvailability.UserId != request.UserId)
                    {
                        _logger.LogInformation("This bike availability doesn't belong to user with UserId {UserId}",
                            request.UserId);
                        return Result<BikeAvailabilityDto>.Failure(
                            $"This bike availability doesn't belong to user with UserId {request.UserId}.");
                    }

                    _logger.LogInformation("Successfully retrieved bike availability");
                    return Result<BikeAvailabilityDto>.Success(bikeAvailability,
                        "Successfully retrieved bike availability.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<BikeAvailabilityDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}