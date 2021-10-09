using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Intimacies.DTOs;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
    public class EditIntimacy
    {
        public class Command : IRequest<Result<Unit>>
        {
            public IntimacyCreateEditDto IntimacyCreateEditDto { get; set; } = null!;
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

                    var oldIntimacy = await _context.Intimacy
                        .FindAsync(new object[]
                        {
                            request.IntimacyCreateEditDto.UserOneId!,
                            request.IntimacyCreateEditDto.UserTwoId!
                        }, cancellationToken);

                    if (oldIntimacy == null) return null!;

                    if (oldIntimacy.IsBlock)
                    {
                        oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
                        oldIntimacy.UnblockTime = CurrentTime.GetCurrentTime();
                    }
                    else if (!oldIntimacy.IsBlock)
                    {
                        oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
                        oldIntimacy.BlockTime = CurrentTime.GetCurrentTime();
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update intimacy of " +
                                               $"userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                                               $"userTwoId {request.IntimacyCreateEditDto.UserTwoId}");
                        return Result<Unit>.Failure("Failed to update intimacy of " +
                                                    $"userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                                                    $"userTwoId {request.IntimacyCreateEditDto.UserTwoId}.");
                    }

                    _logger.LogInformation("Successfully updated intimacy of " +
                                           $"userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                                           $"userTwoId {request.IntimacyCreateEditDto.UserTwoId}");
                    return Result<Unit>.Success(Unit.Value, "Successfully updated intimacy of " +
                                                            $"userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                                                            $"userTwoId {request.IntimacyCreateEditDto.UserTwoId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}