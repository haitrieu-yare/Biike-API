using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.BikeAvailabilities.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.BikeAvailabilities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeAvailabilityEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(BikeAvailabilityModificationDto bikeAvailabilityModificationDto, int userId,
                int bikeAvailabilityId)
            {
                BikeAvailabilityModificationDto = bikeAvailabilityModificationDto;
                UserId = userId;
                BikeAvailabilityId = bikeAvailabilityId;
            }

            public int BikeAvailabilityId { get; }
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

                    var bikeAvailability = await _context.BikeAvailability
                        .Where(b => b.BikeAvailabilityId == request.BikeAvailabilityId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bikeAvailability == null)
                    {
                        _logger.LogInformation("Bike Availability doesn't exist");
                        return Result<Unit>.Failure("Bike Availability doesn't exist.");
                    }

                    if (bikeAvailability.UserId != request.UserId)
                    {
                        _logger.LogInformation("This bike availability doesn't belong to user with UserId {UserId}",
                            request.UserId);
                        return Result<Unit>.Failure(
                            $"This bike availability doesn't belong to user with UserId {request.UserId}.");
                    }

                    _mapper.Map(request.BikeAvailabilityModificationDto, bikeAvailability);

                    var time5Am = new TimeSpan(5, 0, 0);
                    var time9Pm = new TimeSpan(21, 0, 0);

                    if (bikeAvailability.FromTime.TimeOfDay.CompareTo(time5Am) < 0)
                    {
                        _logger.LogInformation("FromTime must be later than 5AM");
                        return Result<Unit>.Failure("FromTime must be later than 5AM.");
                    }

                    if (bikeAvailability.ToTime.TimeOfDay.CompareTo(time9Pm) > 0)
                    {
                        _logger.LogInformation("FromTime must be earlier than 9PM");
                        return Result<Unit>.Failure("FromTime must be earlier than 9PM.");
                    }

                    if (bikeAvailability.FromTime.TimeOfDay.CompareTo(bikeAvailability.ToTime.TimeOfDay) > 0)
                    {
                        _logger.LogInformation("FromTime must be earlier than ToTime");
                        return Result<Unit>.Failure("FromTime must be earlier than ToTime.");
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update bike availability");
                        return Result<Unit>.Failure("Failed to update bike availability.");
                    }

                    _logger.LogInformation("Successfully updated bike availability");
                    return Result<Unit>.Success(Unit.Value, "Successfully updated bike availability.");
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