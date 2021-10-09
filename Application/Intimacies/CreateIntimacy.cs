using System;
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
    public class CreateIntimacy
    {
        public class Command : IRequest<Result<Unit>>
        {
            public IntimacyCreateEditDto IntimacyCreateEditDto { get; set; } = null!;
        }

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

                    var oldIntimacy = await _context.Intimacy
                        .FindAsync(new object[]
                        {
                            request.IntimacyCreateEditDto.UserOneId!,
                            request.IntimacyCreateEditDto.UserTwoId!
                        }, cancellationToken);

                    if (oldIntimacy != null)
                    {
                        _logger.LogInformation("Intimacy has already existed");
                        return Result<Unit>.Failure("Intimacy has already existed.");
                    }

                    Intimacy newIntimacy = new();

                    _mapper.Map(request.IntimacyCreateEditDto, newIntimacy);

                    await _context.Intimacy.AddAsync(newIntimacy, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new intimacy");
                        return Result<Unit>.Failure("Failed to create new intimacy.");
                    }

                    _logger.LogInformation("Successfully created intimacy");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully created intimacy.", newIntimacy.UserOneId.ToString());
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