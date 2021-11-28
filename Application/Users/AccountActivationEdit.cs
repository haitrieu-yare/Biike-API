using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AccountActivationEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int UserId { get; init; }
            public UserActivationDto UserActivationDto { get; init; } = null!;
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

                    User user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User doesn't exist");
                        return Result<Unit>.NotFound("User doesn't exist.");
                    }

                    if (request.UserActivationDto.IsEmailVerified != null)
                    {
                        if (request.UserActivationDto.IsEmailVerified == false)
                        {
                            _logger.LogInformation("Can't set email verification to false");
                            return Result<Unit>.Failure("Can't set email verification to false.");
                        }

                        if (user.IsEmailVerified)
                        {
                            _logger.LogInformation("User with UserId {request.UserId} has already verified email",
                                request.UserId);
                            return Result<Unit>.Failure(
                                $"User with UserId {request.UserId} has already verified email.");
                        }

                        user.IsEmailVerified = (bool) request.UserActivationDto.IsEmailVerified;
                    }

                    if (request.UserActivationDto.IsPhoneVerified != null)
                    {
                        if (request.UserActivationDto.IsPhoneVerified == false)
                        {
                            _logger.LogInformation("Can't set phone verification to false");
                            return Result<Unit>.Failure("Can't set phone verification to false.");
                        }

                        if (user.IsPhoneVerified)
                        {
                            _logger.LogInformation("User with UserId {request.UserId} has already verified phone",
                                request.UserId);
                            return Result<Unit>.Failure(
                                $"User with UserId {request.UserId} has already verified phone.");
                        }

                        user.IsPhoneVerified = (bool) request.UserActivationDto.IsPhoneVerified;
                    }

                    if (user.IsEmailVerified && user.IsPhoneVerified) user.Status = (int) UserStatus.Active;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to verify user with UserId {request.UserId}", request.UserId);
                        return Result<Unit>.Failure($"Failed to verify user with UserId {request.UserId}.");
                    }

                    _logger.LogInformation("Successfully verified user with UserId {request.UserId}", request.UserId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully verified user with UserId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
            }
        }
    }
}