using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    public class BikerPosition
    {
        public class Query : IRequest<Result<BikerPositionDto>>
        {
            public Query(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<BikerPositionDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<BikerPositionDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var user = await _context.User.FindAsync(new object[] {request.UserId}, cancellationToken);

                    if (user == null || user.IsDeleted)
                    {
                        _logger.LogInformation("User with UserId {UserId} doesn't exist", request.UserId);
                        return Result<BikerPositionDto>.NotFound($"User with UserId {request.UserId} doesn't exist.");
                    }

                    if (!user.IsBikeVerified)
                    {
                        _logger.LogInformation("User with UserId {UserId} is not a biker", request.UserId);
                        return Result<BikerPositionDto>.Failure($"User with UserId {request.UserId} is not a biker.");
                    }

                    var bikerPosition = new BikerPositionDto();

                    var bikerChart = await _context.User
                        .Where(u => u.IsBikeVerified == true)
                        .OrderByDescending(u => u.MaxTotalPoint)
                        .Select(u => new {u.UserId, u.MaxTotalPoint})
                        .ToListAsync(cancellationToken);

                    if (bikerChart.Count == 0)
                    {
                        _logger.LogInformation("There are no biker available");
                        return Result<BikerPositionDto>.NotFound("There are no biker available.");
                    }

                    for (var i = 0; i < bikerChart.Count; i++)
                    {
                        if (bikerChart[i].UserId != request.UserId) continue;
                        
                        bikerPosition.ChartPosition = i + 1;
                        bikerPosition.MaxTotalPoint = bikerChart[i].MaxTotalPoint;
                        break;
                    }

                    _logger.LogInformation(
                        "Successfully retrieved user max total point and chart position");
                    return Result<BikerPositionDto>.Success(bikerPosition,
                        "Successfully retrieved user max total point and chart position.");
                }
                catch (Exception e) when (e is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<BikerPositionDto>.Failure("Request was cancelled.");
                }
                catch (Exception e)
                {
                    _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                    return Result<BikerPositionDto>.Failure($"{e.InnerException?.Message ?? e.Message}");
                }
            }
        }
    }
}