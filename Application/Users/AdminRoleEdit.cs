using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Enums;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;
using User = Domain.Entities.User;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdminRoleEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; }
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

                    if (user.IsDeleted)
                    {
                        _logger.LogInformation(
                            "User with UserId {request.UserId} has been deleted. " +
                            "Please reactivate it if you want to edit it", request.UserId);
                        return Result<Unit>.Failure($"User with UserId {request.UserId} has been deleted. " +
                                                    "Please reactivate it if you want to edit it.");
                    }

                    if (user.RoleId == (int) RoleStatus.Keer)
                    {
                        user.RoleId = (int) RoleStatus.Admin;
                    }
                    else
                    {
                        user.RoleId = (int) RoleStatus.Keer;
                    }

                    try
                    {
                        #region Import user's role to Firebase

                        var claims = new Dictionary<string, object> {{"role", user.Role}};

                        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.UserId.ToString(), claims,
                            cancellationToken);

                        #endregion
                    }
                    catch (FirebaseAuthException e)
                    {
                        _logger.LogError("Error create user on Firebase. {Error}",
                            e.InnerException?.Message ?? e.Message);
                        return Result<Unit>.Failure("Error create user on Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}");
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update user's role by userId {request.UserId} to {UserRole}",
                            request.UserId, user.Role);
                        return Result<Unit>.Failure(
                            $"Failed to update user's role by userId {request.UserId} to {user.Role}.");
                    }

                    _logger.LogInformation("Successfully updated user's role by userId {request.UserId} to {UserRole}",
                        request.UserId, user.Role);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated user's role by userId {request.UserId} to {user.Role}.");
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