using System;
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
    public class UserDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int UserId { get; init; }
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

                    if (user == null)
                    {
                        _logger.LogInformation("User doesn't exist");
                        return Result<Unit>.NotFound("User doesn't exist.");
                    }
                    
                    if (user.IsDeleted)
                    {
                        _logger.LogInformation("User with userId {request.UserId} has been deleted. " +
                                               "Please reactivate it to edit it this user", request.UserId);
                        return Result<Unit>.Failure($"User with userId {request.UserId} has been deleted. " +
                                                    "Please reactivate it to edit it this user.");
                    }

                    if (user.RoleId == (int) RoleStatus.Admin)
                    {
                        var adminCount = await _context.User
                            .Where(u => u.RoleId == (int) RoleStatus.Admin)
                            .CountAsync(cancellationToken);

                        if (adminCount == 2)
                        {
                            _logger.LogInformation("Can not delete this user because there are only 2 admin left");
                            return Result<Unit>.Failure("Can not delete this user because there are only 2 admin left.");
                        }
                    }
                    
                    user.IsDeleted = !user.IsDeleted;
                    
                    #region Delete on Firebase

                    try
                    {
                        UserRecordArgs userRecordArgs = new()
                        {
                            Uid = request.UserId.ToString(), Disabled = user.IsDeleted
                        };
                        
                        await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs, cancellationToken);
                    }
                    catch (FirebaseAuthException e)
                    {
                        _logger.LogError("Error delete user on Firebase. {Error}",
                            e.InnerException?.Message ?? e.Message);
                        return Result<Unit>.Failure(
                            $"Error delete user on Firebase. {e.InnerException?.Message ?? e.Message}");
                    }

                    #endregion
                    
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete user with userId {request.UserId}", request.UserId);
                        return Result<Unit>.Failure($"Failed to delete user with userId {request.UserId}.");
                    }

                    _logger.LogInformation("Successfully deleted user with userId {request.UserId}", request.UserId);
                    return Result<Unit>.Success(Unit.Value, $"Successfully deleted user with userId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}