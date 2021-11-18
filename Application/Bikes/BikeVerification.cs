using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using Domain.Enums;
using MediatR;
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
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Bike bike = await _context.Bike.FindAsync(new object[] {request.BikeId}, cancellationToken);

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

                    if (request.VerificationResult)
                    {
                        User user = await _context.User.FindAsync(new object[] {bike.UserId}, cancellationToken);

                        if (user == null || user.IsDeleted)
                        {
                            _logger.LogInformation("User with {UserId} does not exist", bike.UserId);
                            return Result<Unit>.NotFound($"User with {bike.UserId} does not exist.");
                        }

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