using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    public class DeleteFireBaseUser
    {
        public class Command : IRequest<Result<Unit>>
        {
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _logger = logger;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    List<User> users = await _context.User.ToListAsync(cancellationToken);

                    if (users.Count == 0)
                    {
                        _logger.LogInformation("Can't get user's uid to delete on Firebase " +
                                               "because there are no user in database");
                        return Result<Unit>.Failure("Can't get user's uid to delete on Firebase " +
                                                    "because there are no user in database.");
                    }

                    List<string> listUserId = users.Select(user => user.UserId.ToString()).ToList();

                    try
                    {
                        await FirebaseAuth.DefaultInstance.DeleteUsersAsync(listUserId, cancellationToken);
                    }
                    catch (FirebaseAuthException e)
                    {
                        _logger.LogError("Error delete all users on Firebase. {Error}",
                            e.InnerException?.Message ?? e.Message);
                        return Result<Unit>.Failure("Error delete all users on Firebase. " +
                                                    $"{e.InnerException?.Message ?? e.Message}");
                    }

                    _logger.LogInformation("Successfully deleted firebase's users");
                    return Result<Unit>.Success(Unit.Value, "Successfully deleted firebase's users.");
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