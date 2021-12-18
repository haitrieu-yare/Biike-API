using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Notifications;
using Application.Notifications.DTOs;
using Domain;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeVerification
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int bikeId, bool verificationResult, string? failedVerificationReason)
            {
                BikeId = bikeId;
                VerificationResult = verificationResult;
                FailedVerificationReason = failedVerificationReason;
            }

            public int BikeId { get; }
            public bool VerificationResult { get; }
            public string? FailedVerificationReason { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly NotificationSending _notiSender;
            private readonly IConfiguration _configuration;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, NotificationSending notiSender, 
                IConfiguration configuration,ILogger<Handler> logger)
            {
                _context = context;
                _notiSender = notiSender;
                _configuration = configuration;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Bike? bike = await _context.Bike.FindAsync(new object[] {request.BikeId}, cancellationToken);

                    if (bike == null)
                    {
                        _logger.LogInformation("Bike doesn't exist");
                        return Result<Unit>.NotFound("Bike doesn't exist.");
                    }

                    if (bike.Status != (int) BikeStatus.Unverified)
                    {
                        _logger.LogInformation("Bike has already been verified");
                        return Result<Unit>.Failure("Bike has already been verified.");
                    }
                    
                    var user = await _context.User.FindAsync(new object[] {bike.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User with {UserId} does not exist", bike.UserId);
                        return Result<Unit>.NotFound($"User with {bike.UserId} does not exist.");
                    }

                    if (request.VerificationResult)
                    {
                        bike.Status = (int) BikeStatus.SuccessfullyVerified;
                        user.IsBikeVerified = true;
                    }
                    else if (string.IsNullOrEmpty(request.FailedVerificationReason))
                    {
                        _logger.LogInformation("Failed verification need a reason about why it failed");
                        return Result<Unit>.Failure("Failed verification need a reason about why it failed.");
                    }
                    else
                    {
                        bike.Status = (int) BikeStatus.FailedVerified;
                        bike.FailedVerificationReason = request.FailedVerificationReason;
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to verify bike");
                        return Result<Unit>.Failure("Failed to verify bike.");
                    }

                    var verificationResult = request.VerificationResult ? "thành công" : "thất bại";
                    
                    // ReSharper disable StringLiteralTypo
                    var notification = new NotificationDto
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = $"Xác minh xe {verificationResult}",
                        Content = request.VerificationResult
                            ? "Xe của bạn đã được xác minh thành công, bạn có thể bắt đầu nhận chuyến ngay bây giờ"
                            : bike.FailedVerificationReason,
                        ReceiverId = user.UserId,
                        Url = $"{_configuration["ApiPath"]}/bikes",
                        IsRead = false,
                        CreatedDate = CurrentTime.GetCurrentTime()
                    };
                    // ReSharper restore StringLiteralTypo

                    await _notiSender.Run(notification);

                    _logger.LogInformation("Successfully verified bike");
                    return Result<Unit>.Success(Unit.Value, "Successfully verified bike.");
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