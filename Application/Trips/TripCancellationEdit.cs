using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripCancellationEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int TripId { get; init; }
            public int UserId { get; init; }
            public bool IsAdmin { get; init; }
            public TripCancellationDto TripCancellationDto { get; init; } = null!;
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

                    Trip oldTrip = await _context.Trip.FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (oldTrip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.NotFound("Trip doesn't exist.");
                    }

                    if (!request.IsAdmin)
                    {
                        var isUserInTrip = !(oldTrip.BikerId == null && request.UserId != oldTrip.KeerId);

                        if (request.UserId != oldTrip.KeerId && request.UserId != oldTrip.BikerId) isUserInTrip = false;

                        if (!isUserInTrip)
                        {
                            _logger.LogInformation("Cancellation request must come from Keer or Biker of the trip");
                            return Result<Unit>.Failure(
                                "Cancellation request must come from Keer or Biker of the trip.");
                        }
                    }

                    switch (oldTrip.Status)
                    {
                        case (int) TripStatus.Finished:
                            _logger.LogInformation("Trip has already finished");
                            return Result<Unit>.Failure("Trip has already finished.");
                        case (int) TripStatus.Cancelled:
                            _logger.LogInformation("Trip has already cancelled");
                            return Result<Unit>.Failure("Trip has already cancelled.");
                    }

                    _mapper.Map(request.TripCancellationDto, oldTrip);

                    oldTrip.CancelPersonId = request.UserId;
                    oldTrip.Status = (int) TripStatus.Cancelled;
                    oldTrip.FinishedTime = CurrentTime.GetCurrentTime();

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to cancel trip with TripId {request.TripId}", request.TripId);
                        return Result<Unit>.Failure($"Failed to cancel trip with TripId {request.TripId}.");
                    }

                    _logger.LogInformation("Successfully cancelled trip with TripId {request.TripId}", request.TripId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully cancelled trip with TripId {request.TripId}.");
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