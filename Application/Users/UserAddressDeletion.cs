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
            public readonly int AddressId;

            public Command(int userId, int addressId)
            {
                UserId = userId;
                AddressId = addressId;
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

                    var address = await _context.Address.Where(a => a.AddressId == request.AddressId)
                        .Include(a => a.UserAddress)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (address == null)
                    {
                        _logger.LogInformation("Address doesn't exist");
                        return Result<Unit>.NotFound("Address doesn't exist.");
                    }

                    if (address.UserAddress == null)
                    {
                        _logger.LogInformation("UserAddress doesn't exist");
                        return Result<Unit>.NotFound("UserAddress doesn't exist.");
                    }

                    if (address.UserAddress.UserId != request.UserId)
                    {
                        _logger.LogInformation(
                            "Address with AddressId {AddressId} doesn't belong to user with UserId {UserId}",
                            request.AddressId, request.UserId);
                        return Result<Unit>.NotFound(
                            $"Address with AddressId {request.AddressId} doesn't belong to user with UserId {request.UserId}.");
                    }

                    _context.UserAddress.Remove(address.UserAddress);
                    _context.Address.Remove(address);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete user address with addressId {AddressId}",
                            request.AddressId);
                        return Result<Unit>.Failure($"Failed to delete user address with addressId {request.AddressId}.");
                    }

                    _logger.LogInformation("Successfully deleted user address with addressId {AddressId}",
                        request.AddressId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted user address with addressId {request.AddressId}.");
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