using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserKeNowCheck
    {
        public class Query : IRequest<Result<bool>>
        {
            public Query(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<bool>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User with UserId {UserId} doesn't exist", request.UserId);
                        return Result<bool>.NotFound($"User with UserId {request.UserId} doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved user ke now availability by UserId {UserId}",
                        request.UserId);
                    return Result<bool>.Success(user.IsKeNowAvailable,
                        $"Successfully retrieved user ke now availability by UserId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<bool>.Failure("Request was cancelled.");
                }
            }
        }
    }
}