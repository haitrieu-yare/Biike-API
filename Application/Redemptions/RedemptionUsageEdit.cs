using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RedemptionUsageEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int RedemptionId { get; init; }
            public int UserRequestId { get; init; }
        }

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

                    List<Redemption> redemptionsByUserId = await _context.Redemption
                        .Where(r => r.Wallet.User.UserId == request.UserRequestId)
                        .ToListAsync(cancellationToken);

                    if (redemptionsByUserId.Count == 0)
                    {
                        _logger.LogInformation("User with {request.UserRequestId} doesn't have any redemption",
                            request.UserRequestId);
                        return Result<Unit>.NotFound($"User with {request.UserRequestId} doesn't have any redemption.");
                    }

                    var redemption =
                        redemptionsByUserId.Find(redemption => redemption.RedemptionId == request.RedemptionId);

                    if (redemption == null)
                    {
                        _logger.LogInformation(
                            "Redemption doesn't exist or this redemption doesn't belong to user with {request.UserRequestId}",
                            request.UserRequestId);
                        return Result<Unit>.NotFound(
                            $"Redemption doesn't exist or this redemption doesn't belong to user with {request.UserRequestId}.");
                    }

                    redemption.IsUsed = !redemption.IsUsed;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to update redemption usage with RedemptionId {request.RedemptionId}",
                            request.RedemptionId);
                        return Result<Unit>.Failure("Failed to update redemption usage " +
                                                    $"with RedemptionId {request.RedemptionId}.");
                    }

                    _logger.LogInformation(
                        "Successfully updated redemption usage with RedemptionId {request.RedemptionId}",
                        request.RedemptionId);
                    return Result<Unit>.Success(Unit.Value,
                        "Successfully updated redemption usage " + $"with RedemptionId {request.RedemptionId}.");
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