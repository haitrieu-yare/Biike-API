using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserAddressCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public readonly int UserId;
            public readonly UserAddressCreationDto UserAddressCreationDto;

            public Command(int userId, UserAddressCreationDto userAddressCreationDto)
            {
                UserId = userId;
                UserAddressCreationDto = userAddressCreationDto;
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

                    await using IDbContextTransaction transaction =
                        await _context.Database.BeginTransactionAsync(cancellationToken);

                    var address = new Address
                    {
                        AddressName = request.UserAddressCreationDto.AddressName!,
                        AddressDetail = request.UserAddressCreationDto.AddressDetail!,
                        AddressCoordinate = request.UserAddressCreationDto.AddressCoordinate!
                    };

                    await _context.Address.AddAsync(address, cancellationToken);
                    var resultAddress = await _context.SaveChangesAsync(cancellationToken) > 0;

                    var userAddressesCount = await _context.UserAddress.Where(u => u.UserId == request.UserId)
                        .CountAsync(cancellationToken);

                    var userAddress = new UserAddress
                    {
                        UserId = request.UserId,
                        AddressId = address.AddressId,
                        Note = request.UserAddressCreationDto.Note,
                        IsDefault = userAddressesCount == 0
                    };

                    await _context.UserAddress.AddAsync(userAddress, cancellationToken);
                    var resultUserAddress = await _context.SaveChangesAsync(cancellationToken) > 0;

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    await transaction.CommitAsync(cancellationToken);

                    if (!resultAddress || !resultUserAddress)
                    {
                        _logger.LogInformation("Failed to create new user address");
                        return Result<Unit>.Failure("Failed to create new user address.");
                    }

                    _logger.LogInformation("Successfully create new user address");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new user address.");
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