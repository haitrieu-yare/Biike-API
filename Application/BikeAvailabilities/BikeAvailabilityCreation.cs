using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BikeAvailabilities.DTOs;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.BikeAvailabilities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeAvailabilityCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(BikeAvailabilityModificationDto bikeAvailabilityModificationDto, int userId)
            {
                BikeAvailabilityModificationDto = bikeAvailabilityModificationDto;
                UserId = userId;
            }

            public BikeAvailabilityModificationDto BikeAvailabilityModificationDto { get; }
            public int UserId { get; }
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

                    BikeAvailability newBikeAvailability = new();

                    _mapper.Map(request.BikeAvailabilityModificationDto, newBikeAvailability);

                    newBikeAvailability.UserId = request.UserId;

                    await _context.BikeAvailability.AddAsync(newBikeAvailability, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new bike availability");
                        return Result<Unit>.Failure("Failed to create new bike availability.");
                    }

                    _logger.LogInformation("Successfully created bike availability");
                    return Result<Unit>.Success(Unit.Value, "Successfully created bike availability.",
                        newBikeAvailability.BikeAvailabilityId.ToString());
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