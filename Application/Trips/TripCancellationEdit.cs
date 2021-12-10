using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications;
using Application.Notifications.DTOs;
using Application.Trips.DTOs;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripCancellationEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int tripId, int userId, bool isAdmin, TripCancellationDto tripCancellationDto)
            {
                TripId = tripId;
                UserId = userId;
                IsAdmin = isAdmin;
                TripCancellationDto = tripCancellationDto;
            }
            
            public int TripId { get; }
            public int UserId { get; }
            public bool IsAdmin { get; }
            public TripCancellationDto TripCancellationDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly NotificationSending _notiSender;
            private readonly TripCancellationCheck _tripCancellationCheck;
            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, IMapper mapper, IConfiguration configuration, 
                ILogger<Handler> logger, NotificationSending notiSender, TripCancellationCheck tripCancellationCheck)
            {
                _context = context;
                _mapper = mapper;
                _configuration = configuration;
                _logger = logger;
                _notiSender = notiSender;
                _tripCancellationCheck = tripCancellationCheck;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    User user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User with {UserId} doesn't exist", request.UserId);
                        return Result<Unit>.NotFound($"User with {request.UserId} doesn't exist.");
                    }
                    
                    if (await _tripCancellationCheck.IsLimitExceeded(user.UserId))
                    {
                        _logger.LogInformation("You have exceeded the maximum number of cancellation in one day");
                        return Result<Unit>.Failure("You have exceeded the maximum number of cancellation in one day.");
                    }
                    
                    Trip trip = await _context.Trip.FindAsync(new object[] {request.TripId}, cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<Unit>.NotFound("Trip doesn't exist.");
                    }

                    if (!request.IsAdmin)
                    {
                        var isUserInTrip = !(trip.BikerId == null && request.UserId != trip.KeerId);

                        if (request.UserId != trip.KeerId && request.UserId != trip.BikerId) isUserInTrip = false;

                        if (!isUserInTrip)
                        {
                            _logger.LogInformation("Cancellation request must come from Keer or Biker of the trip");
                            return Result<Unit>.Failure(
                                "Cancellation request must come from Keer or Biker of the trip.");
                        }
                    }

                    switch (trip.Status)
                    {
                        case (int) TripStatus.Finished:
                            _logger.LogInformation("Trip has already finished");
                            return Result<Unit>.Failure("Trip has already finished.");
                        case (int) TripStatus.Cancelled:
                            _logger.LogInformation("Trip has already cancelled");
                            return Result<Unit>.Failure("Trip has already cancelled.");
                    }

                    _mapper.Map(request.TripCancellationDto, trip);

                    trip.CancelPersonId = request.UserId;
                    trip.Status = (int) TripStatus.Cancelled;
                    trip.CancelTime = CurrentTime.GetCurrentTime();

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to cancel trip with TripId {request.TripId}", request.TripId);
                        return Result<Unit>.Failure($"Failed to cancel trip with TripId {request.TripId}.");
                    }
                    
                    var isKeer = request.UserId == trip.KeerId;
                    if (trip.BikerId != null)
                    {
                        // ReSharper disable StringLiteralTypo
                        var notification = new NotificationDto
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = "Chuyến đi đã bị hủy",
                            Content = $"Chuyến đi vào {trip.BookTime} đã bị hủy bởi {user.FullName}",
                            ReceiverId = isKeer ? trip.BikerId : trip.KeerId,
                            Url = $"{_configuration["ApiPath"]}/trips/{trip.TripId}/details",
                            IsRead = false,
                            CreatedDate = CurrentTime.GetCurrentTime()
                        };
                        // ReSharper restore StringLiteralTypo

                        await _notiSender.Run(notification);
                    }
                    else if (request.IsAdmin)
                    {
                        // ReSharper disable StringLiteralTypo
                        await _notiSender.Run(new NotificationDto
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = "Chuyến đi đã bị hủy",
                            Content = $"Chuyến đi vào {trip.BookTime} đã bị hủy bởi Admin {user.FullName}",
                            ReceiverId = trip.KeerId,
                            Url = $"{_configuration["ApiPath"]}/trips/{trip.TripId}/details",
                            IsRead = false,
                            CreatedDate = CurrentTime.GetCurrentTime()
                        });
                        // ReSharper restore StringLiteralTypo

                        if (trip.BikerId != null)
                        {
                            // ReSharper disable StringLiteralTypo
                            await _notiSender.Run(new NotificationDto
                            {
                                NotificationId = Guid.NewGuid(),
                                Title = "Chuyến đi đã bị hủy",
                                Content = $"Chuyến đi vào {trip.BookTime} đã bị hủy bởi Admin {user.FullName}",
                                ReceiverId = trip.BikerId,
                                Url = $"{_configuration["ApiPath"]}/trips/{trip.TripId}/details",
                                IsRead = false,
                                CreatedDate = CurrentTime.GetCurrentTime()
                            });
                            // ReSharper restore StringLiteralTypo
                        }
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
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}