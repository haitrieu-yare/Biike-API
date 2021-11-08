using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserAddressCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public readonly UserAddressCreationDto UserAddressCreationDto;

            public Command(UserAddressCreationDto userAddressCreationDto)
            {
                UserAddressCreationDto = userAddressCreationDto;
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

                    var userAddressesCount = await _context.UserAddress
                        .Where(u => u.UserId == request.UserAddressCreationDto.UserId)
                        .CountAsync(cancellationToken);

                    var userAddress = new UserAddress
                    {
                        IsDefault = userAddressesCount == 0
                    };

                    _mapper.Map(request.UserAddressCreationDto, userAddress);

                    await _context.UserAddress.AddAsync(userAddress, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
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