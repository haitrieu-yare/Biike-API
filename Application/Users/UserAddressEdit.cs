using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserAddressEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public readonly int UserId;
            public readonly int AddressId;
            public readonly UserAddressDto UserAddressDto;

            public Command(int userId, int addressId, UserAddressDto userAddressDto)
            {
                UserId = userId;
                UserAddressDto = userAddressDto;
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

                if (request.UserAddressDto.IsDefault == false)
                {
                    _logger.LogInformation("IsDefault doesn't accept false value");
                    return Result<Unit>.NotFound("IsDefault doesn't accept false value.");
                }

                if (request.UserAddressDto.IsDefault != null)
                {
                    var defaultUserAddress = await _context.UserAddress.Where(u => u.UserId == request.UserId)
                        .Where(u => u.IsDefault == true)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (defaultUserAddress.UserAddressId == address.UserAddress.UserAddressId)
                    {
                        _logger.LogInformation(
                            "Address with AddressId {AddressId} is already a default for user " +
                            "with userId {UserId}", request.AddressId, request.UserId);
                        return Result<Unit>.NotFound(
                            $"Address with AddressId {request.AddressId} is already a default " +
                            $"for with userId {request.UserId}.");
                    }

                    address.UserAddress.IsDefault = request.UserAddressDto.IsDefault.Value;
                    defaultUserAddress.IsDefault = false;
                }

                if (request.UserAddressDto.AddressName != null)
                    address.AddressName = request.UserAddressDto.AddressName;

                if (request.UserAddressDto.AddressDetail != null)
                    address.AddressDetail = request.UserAddressDto.AddressDetail;
                
                if (request.UserAddressDto.AddressCoordinate != null)
                    address.AddressCoordinate = request.UserAddressDto.AddressCoordinate;

                if (request.UserAddressDto.Note != null) address.UserAddress.Note = request.UserAddressDto.Note;

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    _logger.LogInformation("Failed to update user address with addressId {AddressId}",
                        request.AddressId);
                    return Result<Unit>.Failure($"Failed to update user address with addressId {request.AddressId}.");
                }

                _logger.LogInformation("Successfully updated user address with addressId {AddressId}",
                    request.AddressId);
                return Result<Unit>.Success(Unit.Value,
                    $"Successfully updated user address with addressId {request.AddressId}.");
            }
        }
    }
}