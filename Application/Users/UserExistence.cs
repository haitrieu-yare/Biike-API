using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserExistence
    {
        public class Command : IRequest<Result<Unit>>
        {
            public UserExistenceDto UserExistenceDto { get; init; } = null!;
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

                    Domain.Entities.User user = await _context.User
                        .Where(u => u.Email == request.UserExistenceDto.Email ||
                                    u.PhoneNumber == request.UserExistenceDto.PhoneNumber)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (user != null)
                    {
                        _logger.LogInformation("User with the same email or phone number has already existed");
                        return Result<Unit>.Failure("User with the same email or phone number has already existed.");
                    }

                    _logger.LogInformation("User doesn't exist");
                    return Result<Unit>.Success(Unit.Value, "User doesn't exist.");
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