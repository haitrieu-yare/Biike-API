using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public BikeCreationDto BikeCreationDto { get; init; } = null!;
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

                    User user = await _context.User
                        .FindAsync(new object[] {request.BikeCreationDto.UserId!}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User to create bike does not exist");
                        return Result<Unit>.NotFound("User to create bike does not exist.");
                    }

                    user.IsBikeVerified = true;

                    Bike oldBike = await _context.Bike
                        .Where(b => b.UserId == request.BikeCreationDto.UserId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (oldBike != null)
                    {
                        _logger.LogInformation("User already has a bike");
                        return Result<Unit>.Failure("User already has a bike.");
                    }

                    Bike newBike = new();

                    _mapper.Map(request.BikeCreationDto, newBike);

                    await _context.Bike.AddAsync(newBike, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new bike");
                        return Result<Unit>.Failure("Failed to create new bike.");
                    }

                    _logger.LogInformation("Successfully created new bike");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully created new bike.", newBike.BikeId.ToString());
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