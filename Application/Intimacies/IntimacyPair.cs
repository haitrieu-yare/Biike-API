using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IntimacyPair
    {
        public class Query : IRequest<Result<bool>>
        {
            public Query(int userOneId, int userTwoId)
            {
                UserOneId = userOneId;
                UserTwoId = userTwoId;
            }

            public int UserOneId { get; }
            public int UserTwoId { get; }
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

                    var intimacy = await _context.Intimacy
                        .Where(i => i.UserOneId == request.UserOneId)
                        .Where(i => i.UserTwoId == request.UserTwoId)
                        .SingleOrDefaultAsync(cancellationToken);

                    _logger.LogInformation(
                        "Successfully retrieved result of intimacy by UserOneId {UserOneId} and" +
                        " UserTwoId {UserTwoId}", request.UserOneId, request.UserTwoId);

                    return Result<bool>.Success(intimacy != null,
                        $"Successfully retrieved result of intimacy by UserOneId {request.UserOneId}" +
                        $" and UserTwoId {request.UserTwoId}.");
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