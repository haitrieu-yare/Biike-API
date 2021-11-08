using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
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
            public readonly int UserAddressId;
            public readonly UserAddressDto UserAddressDto;

            public Command(int userAddressId, UserAddressDto userAddressDto)
            {
                UserAddressId = userAddressId;
                UserAddressDto = userAddressDto;
            }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

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

                    var userAddress = await _context.UserAddress
                        .Where(a => a.UserAddressId == request.UserAddressId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (userAddress == null)
                    {
                        _logger.LogInformation("UserAddress doesn't exist");
                        return Result<Unit>.NotFound("UserAddress doesn't exist.");
                    }

                    if (request.UserAddressDto.IsDefault != null)
                    {
                        if (request.UserAddressDto.IsDefault == false)
                        {
                            _logger.LogInformation("IsDefault doesn't accept false value");
                            return Result<Unit>.NotFound("IsDefault doesn't accept false value.");
                        }

                        var defaultUserAddress = await _context.UserAddress
                            .Where(u => u.UserId == request.UserAddressDto.UserId)
                            .Where(u => u.IsDefault == true)
                            .SingleOrDefaultAsync(cancellationToken);

                        if (defaultUserAddress.UserAddressId == request.UserAddressId)
                        {
                            _logger.LogInformation(
                                "UserAddress with UserAddressId {AddressId} is already a default for user " +
                                "with UserId {UserId}", request.UserAddressId,
                                request.UserAddressDto.UserId);
                            return Result<Unit>.NotFound(
                                $"Address with AddressId {request.UserAddressId} is already a default " +
                                $"for with UserId {request.UserAddressDto.UserId}.");
                        }

                        userAddress.IsDefault = request.UserAddressDto.IsDefault.Value;
                        defaultUserAddress.IsDefault = false;
                    }

                    _mapper.Map(request.UserAddressDto, userAddress);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update user address with UserAddressId {UserAddressId}",
                            request.UserAddressId);
                        return Result<Unit>.Failure(
                            $"Failed to update user address with UserAddressId {request.UserAddressId}.");
                    }

                    _logger.LogInformation("Successfully updated user address with UserAddressId {UserAddressId}",
                        request.UserAddressId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated user address with UserAddressId {request.UserAddressId}.");
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