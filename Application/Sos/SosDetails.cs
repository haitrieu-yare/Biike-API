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
    public class SosDetails
    {
        public class Query : IRequest<Result<SosDto>>
        {
            public Query(int sosId)
            {
                SosId = sosId;
            }

            public int SosId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<SosDto>>
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

            public async Task<Result<SosDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    SosDto sos = new();

                    Domain.Entities.Sos sosDb =
                        await _context.Sos.FindAsync(new object[] {request.SosId}, cancellationToken);

                    if (sosDb == null)
                    {
                        _logger.LogInformation("Sos doesn't exist");
                        return Result<SosDto>.NotFound("Sos doesn't exist.");
                    }

                    _mapper.Map(sosDb, sos);

                    _logger.LogInformation("Successfully retrieved sos by sosId {request.SosId}", request.SosId);
                    return Result<SosDto>.Success(sos, $"Successfully retrieved sos by sosId {request.SosId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<SosDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}