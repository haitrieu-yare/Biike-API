using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TopBiker
    {
        public class Query : IRequest<Result<List<UserDto>>>
        {
        }

        // ReSharper disable once UnusedType.Local
        private class Handler : IRequestHandler<Query, Result<List<UserDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<List<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bikers = await _context.User
                        .Where(u => u.IsBikeVerified == true)
                        .OrderByDescending(u => u.MaxTotalPoint)
                        .Take(10)
                        .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                        .ToListAsync(cancellationToken);
                    
                    bikers.ForEach(b =>
                    {
                        b.CreatedDate = null;
                        b.IsDeleted = null;
                    });

                    _logger.LogInformation(
                        "Successfully retrieved list of top 10 highest total points in 1 months bikers");
                    return Result<List<UserDto>>.Success(bikers,
                        "Successfully retrieved list of top 10 highest total points in 1 months bikers.");
                }
                catch (Exception e) when (e is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<UserDto>>.Failure("Request was cancelled.");
                }
                catch (Exception e)
                {
                    _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                    return Result<List<UserDto>>.Failure($"{e.InnerException?.Message ?? e.Message}");
                }
            }
        }
    }
}