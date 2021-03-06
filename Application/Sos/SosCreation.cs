using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Sos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Sos
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SosCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(SosCreationDto sosCreationDto, int userId)
            {
                SosCreationDto = sosCreationDto;
                UserId = userId;
            }

            public SosCreationDto SosCreationDto { get; }
            public int UserId { get; }
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

                    var totalRecord = await _context.Sos.Where(s => s.UserId == request.UserId)
                        .CountAsync(cancellationToken);

                    if (totalRecord == 3)
                    {
                        _logger.LogInformation("This user has reach the maximum number of sos (3) can be created");
                        return Result<Unit>.Failure(
                            "This user has reach the maximum number of sos (3) can be created.");
                    }

                    Domain.Entities.Sos newSos = new();

                    _mapper.Map(request.SosCreationDto, newSos);

                    newSos.UserId = request.UserId;

                    await _context.Sos.AddAsync(newSos, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new sos");
                        return Result<Unit>.Failure("Failed to create new sos.");
                    }

                    _logger.LogInformation("Successfully created new sos");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new sos.", newSos.SosId.ToString());
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