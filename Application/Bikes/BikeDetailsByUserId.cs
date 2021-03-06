using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BikeDetailsByUserId
    {
        public class Query : IRequest<Result<BikeDto>>
        {
            public int UserId { get; init; }
            public bool IsAdmin { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<BikeDto>>
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

            public async Task<Result<BikeDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    BikeDto? bike = await _context.Bike
                        .Where(b => b.UserId == request.UserId)
                        .ProjectTo<BikeDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (bike == null)
                    {
                        _logger.LogInformation("Could not found bike with UserId {request.UserId}", request.UserId);
                        return Result<BikeDto>.NotFound($"Could not found bike with UserId {request.UserId}.");
                    }

                    if (!request.IsAdmin)
                        // Set to null to make unnecessary fields excluded from the response body.
                        bike.CreatedDate = null;

                    _logger.LogInformation("Successfully retrieved bike by UserId {request.UserId}", request.UserId);
                    return Result<BikeDto>.Success(bike, $"Successfully retrieved bike by UserId {request.UserId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<BikeDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}