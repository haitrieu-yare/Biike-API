using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
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

                    Domain.Entities.User user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null)
                    {
                        _logger.LogInformation("User doesn't exist");
                        return Result<Unit>.NotFound("User doesn't exist.");
                    }

                    UserRecordArgs userRecordArgs = new()
                    {
                        Uid = request.UserId.ToString(), Disabled = !user.IsDeleted
                    };

                    #region Delete on Firebase

                    try
                    {
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

                    user.IsDeleted = !user.IsDeleted;

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