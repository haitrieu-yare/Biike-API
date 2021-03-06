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

namespace Application.Bikes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeDeletion
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

                    User? user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User to delete bike does not exist");
                        return Result<Unit>.NotFound("User to delete bike does not exist.");
                    }

                    user.IsBikeVerified = false;
                    user.RoleId = (int) RoleStatus.Keer;

                    Bike? bike = await _context.Bike
                        .Where(b => b.UserId == request.UserId)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bike == null)
                    {
                        _logger.LogInformation("Bike does not exist");
                        return Result<Unit>.NotFound("Bike does not exist.");
                    }

                    _context.Bike.Remove(bike);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete bike by userId {UserId}", request.UserId);
                        return Result<Unit>.Failure($"Failed to delete bike by userId {request.UserId}.");
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

                    _logger.LogInformation("Successfully deleted bike by userId {UserId}", request.UserId);
                    return Result<Unit>.Success(Unit.Value, 
                        $"Successfully deleted bike by userId {request.UserId}.");
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