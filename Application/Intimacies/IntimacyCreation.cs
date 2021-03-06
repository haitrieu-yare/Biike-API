using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Intimacies.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IntimacyCreation
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
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Intimacy? oldIntimacy = await _context.Intimacy.FindAsync(
                        new object[]
                        {
                            request.IntimacyModificationDto.UserOneId!, request.IntimacyModificationDto.UserTwoId!
                        }, cancellationToken);

                    if (oldIntimacy != null)
                    {
                        _logger.LogInformation("Intimacy has already existed");
                        return Result<Unit>.Failure("Intimacy has already existed.");
                    }

                    User? userTwo = await _context.User
                        .Where(u => u.UserId == request.IntimacyModificationDto.UserTwoId)
                        .Where(u => u.IsDeleted != true)
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (userTwo == null)
                    {
                        _logger.LogInformation("User two with UserId {UserId} doesn't exist",
                            request.IntimacyModificationDto.UserTwoId);
                        return Result<Unit>.NotFound(
                            $"User two with UserId {request.IntimacyModificationDto.UserTwoId} doesn't exist.");
                    }
                    
                    Intimacy newIntimacy = new();

                    _mapper.Map(request.IntimacyModificationDto, newIntimacy);

                    newIntimacy.UserName = userTwo.FullName;

                    await _context.Intimacy.AddAsync(newIntimacy, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new intimacy");
                        return Result<Unit>.Failure("Failed to create new intimacy.");
                    }

                    _logger.LogInformation("Successfully created intimacy");
                    return Result<Unit>.Success(Unit.Value, "Successfully created intimacy.",
                        newIntimacy.UserOneId.ToString());
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