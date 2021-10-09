using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
    public class EditUsageRedemption
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int RedemptionId { get; set; }
            public int UserRequestId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<EditUsageRedemption> _logger;

            public Handler(DataContext context, ILogger<EditUsageRedemption> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var redemption = await _context.Redemption
                        .Where(r => r.RedemptionId == request.RedemptionId)
                        .Where(r => r.Wallet.User.UserId == request.UserRequestId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (redemption == null) return null!;

                    redemption.IsUsed = !redemption.IsUsed;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update redemption usage " +
                                               $"with RedemptionId {request.RedemptionId}");
                        return Result<Unit>.Failure("Failed to update redemption usage " +
                                                    $"with RedemptionId {request.RedemptionId}.");
                    }

                    _logger.LogInformation("Successfully updated redemption usage " +
                                           $"with RedemptionId {request.RedemptionId}");
                    return Result<Unit>.Success(Unit.Value, "Successfully updated redemption usage " +
                                                            $"with RedemptionId {request.RedemptionId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}