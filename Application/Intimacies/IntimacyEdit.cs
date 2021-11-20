using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Intimacies.DTOs;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IntimacyEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public IntimacyModificationDto IntimacyModificationDto { get; init; } = null!;
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

                    Intimacy oldIntimacy = await _context.Intimacy.FindAsync(
                        new object[]
                        {
                            request.IntimacyModificationDto.UserOneId!, request.IntimacyModificationDto.UserTwoId!
                        }, cancellationToken);

                    if (oldIntimacy == null)
                    {
                        _logger.LogInformation("Intimacy doesn't exist");
                        return Result<Unit>.NotFound("Intimacy doesn't exist.");
                    }

                    switch (oldIntimacy.IsBlock)
                    {
                        case true:
                            oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
                            oldIntimacy.UnblockTime = CurrentTime.GetCurrentTime();
                            break;
                        case false:
                            oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
                            oldIntimacy.BlockTime = CurrentTime.GetCurrentTime();
                            break;
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to update intimacy of " +
                            "userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                            "userTwoId {request.IntimacyCreateEditDto.UserTwoId}",
                            request.IntimacyModificationDto.UserOneId, request.IntimacyModificationDto.UserTwoId);
                        return Result<Unit>.Failure("Failed to update intimacy of " +
                                                    $"userOneId {request.IntimacyModificationDto.UserOneId} and " +
                                                    $"userTwoId {request.IntimacyModificationDto.UserTwoId}.");
                    }

                    _logger.LogInformation(
                        "Successfully updated intimacy of " +
                        "userOneId {request.IntimacyCreateEditDto.UserOneId} and " +
                        "userTwoId {request.IntimacyCreateEditDto.UserTwoId}", request.IntimacyModificationDto.UserOneId,
                        request.IntimacyModificationDto.UserTwoId);
                    return Result<Unit>.Success(Unit.Value,
                        "Successfully updated intimacy of " +
                        $"userOneId {request.IntimacyModificationDto.UserOneId} and " +
                        $"userTwoId {request.IntimacyModificationDto.UserTwoId}.");
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