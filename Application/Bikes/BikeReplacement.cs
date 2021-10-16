using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeReplacement
    {
        public class Command : IRequest<Result<Unit>>
        {
            public BikeCreationDto BikeCreationDto { get; init; } = null!;
        }

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

                    Domain.Entities.Bike oldBike = await _context.Bike
                        .Where(b => b.UserId == request.BikeCreationDto.UserId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (oldBike == null)
                    {
                        _logger.LogInformation("User doesn't have a bike");
                        return Result<Unit>.Failure("User doesn't have a bike.");
                    }

                    _context.Bike.Remove(oldBike);

                    Domain.Entities.Bike newBike = new();

                    _mapper.Map(request.BikeCreationDto, newBike);

                    await _context.Bike.AddAsync(newBike, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to replace old bike with new bike");
                        return Result<Unit>.Failure("Failed to replace old bike with new bike.");
                    }

                    _logger.LogInformation("Successfully replace old bike with new bike");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully replace old bike with new bike.", newBike.BikeId.ToString());
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}