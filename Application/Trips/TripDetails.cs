using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TripDetails
    {
        public class Query : IRequest<Result<TripDetailsDto>>
        {
            public int TripId { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<TripDetailsDto>>
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

            public async Task<Result<TripDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    TripDetailsDto trip = await _context.Trip.Where(t => t.TripId == request.TripId)
                        .ProjectTo<TripDetailsDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (trip == null)
                    {
                        _logger.LogInformation("Trip doesn't exist");
                        return Result<TripDetailsDto>.NotFound("Trip doesn't exist");
                    }

                    _logger.LogInformation("Successfully retrieved trip by TripId {request.TripId}", request.TripId);
                    return Result<TripDetailsDto>.Success(trip,
                        $"Successfully retrieved trip by TripId {request.TripId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<TripDetailsDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}