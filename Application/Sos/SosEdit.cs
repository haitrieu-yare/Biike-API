using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Sos.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Sos
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SosEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int sosId, SosDto newSosDto, int userId)
            {
                SosId = sosId;
                NewSosDto = newSosDto;
                UserId = userId;
            }
            
            public int SosId { get; }
            public SosDto NewSosDto { get; }
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

                    Domain.Entities.Sos? oldSos = await _context.Sos
                        .FindAsync(new object[] {request.SosId}, cancellationToken);

                    if (oldSos == null)
                    {
                        _logger.LogInformation("Sos doesn't exist");
                        return Result<Unit>.NotFound("Sos doesn't exist.");
                    }

                    if (oldSos.UserId != request.UserId)
                    {
                        _logger.LogInformation("This sos doesn't belong to this user");
                        return Result<Unit>.Failure("This sos doesn't belong to this user.");
                    }

                    _mapper.Map(request.NewSosDto, oldSos);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update sos by sosId {request.SosId}",
                            request.SosId);
                        return Result<Unit>.Failure($"Failed to update sos by sosId {request.SosId}.");
                    }

                    _logger.LogInformation("Successfully updated sos by sosId {request.SosId}",
                        request.SosId);
                    return Result<Unit>.Success(
                        Unit.Value, $"Successfully updated sos by sosId {request.SosId}.");
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