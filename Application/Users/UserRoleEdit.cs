using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using Domain.Enums;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserRoleEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int UserId { get; init; }
            public int StartupRole { get; init; }
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

                    User? user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User doesn't exist");
                        return Result<Unit>.NotFound("User doesn't exist.");
                    }

                    bool isRoleIdEqualStartupRole = false;

                    if (request.StartupRole <= 0)
                    {
                        if (user.RoleId == (int) RoleStatus.Keer && !user.IsBikeVerified)
                        {
                            var bike = await _context.Bike
                                .Where(b => b.UserId == user.UserId)
                                .SingleOrDefaultAsync(cancellationToken);
                            
                            if (bike == null)
                            {
                                _logger.LogInformation("User does not have bike");
                                return Result<Unit>.Failure("User does not have bike.");
                            }
                            
                            _logger.LogInformation("User's bike has not been verified");
                            return Result<Unit>.Failure("User's bike has not been verified.");
                        }

                        switch (user.RoleId)
                        {
                            case (int) RoleStatus.Keer:
                                user.RoleId = (int) RoleStatus.Biker;
                                break;
                            case (int) RoleStatus.Biker:
                                user.RoleId = (int) RoleStatus.Keer;
                                break;
                        }
                    }
                    else
                    {
                        if (request.StartupRole == (int) RoleStatus.Biker && 
                            user.RoleId == (int) RoleStatus.Keer && !user.IsBikeVerified)
                        {
                            var bike = await _context.Bike
                                .Where(b => b.UserId == user.UserId)
                                .SingleOrDefaultAsync(cancellationToken);
                            
                            if (bike == null)
                            {
                                _logger.LogInformation("User does not have bike");
                                return Result<Unit>.Failure("User does not have bike.");
                            }
                            
                            _logger.LogInformation("User's bike has not been verified");
                            return Result<Unit>.Failure("User's bike has not been verified.");
                        }

                        if (user.RoleId == request.StartupRole)
                        {
                            isRoleIdEqualStartupRole = true;
                        }
                        else
                        {
                            user.RoleId = request.StartupRole;
                        }
                    }

                    try
                    {
                        #region Import user's role to Firebase

                        var claims = new Dictionary<string, object> {{"role", user.RoleId}};

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

                    if (request.StartupRole <= 0 || request.StartupRole > 0 && !isRoleIdEqualStartupRole)
                    {
                        var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            _logger.LogInformation("Failed to update user's role by userId {UserId} to {UserRole}",
                                request.UserId, user.RoleId);
                            return Result<Unit>.Failure($"Failed to update user's role by userId {request.UserId} to {user.RoleId}.");
                        }
                    }

                    _logger.LogInformation("Successfully updated user's role by userId {UserId} to {UserRole}",
                        request.UserId, user.RoleId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated user's role by userId {request.UserId} to {user.RoleId}.");
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