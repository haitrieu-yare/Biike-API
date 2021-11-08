using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserAddressDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public readonly int UserId;
            public readonly int UserAddressId;

            public Command(int userId, int userAddressId)
            {
                UserId = userId;
                UserAddressId = userAddressId;
            }
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

                    var userAddress = await _context.UserAddress.Where(u => u.UserAddressId == request.UserAddressId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (userAddress == null)
                    {
                        _logger.LogInformation("Address doesn't exist");
                        return Result<Unit>.NotFound("Address doesn't exist.");
                    }

                    if (userAddress.UserId != request.UserId)
                    {
                        _logger.LogInformation(
                            "UserAddress with UserAddressId {UserAddressId} doesn't belong to user with UserId {UserId}",
                            request.UserAddressId, request.UserId);
                        return Result<Unit>.NotFound(
                            $"UserAddress with UserAddressId {request.UserAddressId} doesn't belong to user with UserId {request.UserId}");
                    }

                    _context.UserAddress.Remove(userAddress);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete user address with UserAddressId {UserAddressId}",
                            request.UserAddressId);
                        return Result<Unit>.Failure(
                            $"Failed to delete user address with UserAddressId {request.UserAddressId}.");
                    }

                    _logger.LogInformation("Successfully deleted user address with UserAddressId {UserAddressId}",
                        request.UserAddressId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted user address with UserAddressId {request.UserAddressId}.");
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