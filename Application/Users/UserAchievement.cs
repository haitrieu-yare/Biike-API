using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserAchievement
    {
        public class Query : IRequest<Result<UserAchievementDto>>
        {
            public Query(int userId)
            {
                UserId = userId;
            }
            
            public int UserId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<UserAchievementDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<UserAchievementDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var totalKeerTrip = await _context.Trip
                        .Where(t => t.KeerId == request.UserId)
                        .Where(t => t.Status == (int) TripStatus.Finished)
                        .CountAsync(cancellationToken);
                    
                    var totalBikerTrip = await _context.Trip
                        .Where(t => t.BikerId == request.UserId)
                        .Where(t => t.Status == (int) TripStatus.Finished)
                        .CountAsync(cancellationToken);

                    var totalKmSaved = await _context.Trip
                        .Include(t => t.Route)
                        .Where(t => t.KeerId == request.UserId || t.BikerId == request.UserId)
                        .Where(t => t.Status == (int) TripStatus.Finished)
                        .Select(t => t.Route.Distance)
                        .SumAsync(cancellationToken);

                    var fuelConversion = await _context.Configuration
                        .Where(c => c.ConfigurationName.Equals("FuelConversion"))
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (fuelConversion == null)
                    {
                        _logger.LogInformation("Can not find fuel conversion configuration");
                        return Result<UserAchievementDto>.Failure("Can not find fuel conversion configuration.");
                    }

                    var totalFuelSaved = totalKmSaved * Convert.ToDouble(fuelConversion.ConfigurationValue);

                    var userAchievementDto = new UserAchievementDto
                    {
                        UserId = request.UserId,
                        TotalKeerTrip = totalKeerTrip,
                        TotalBikerTrip = totalBikerTrip,
                        TotalKmSaved = totalKmSaved,
                        TotalFuelSaved = totalFuelSaved 
                    };

                    _logger.LogInformation("Successfully retrieved user self profile");
                    return Result<UserAchievementDto>.Success(
                        userAchievementDto, "Successfully retrieved user self profile.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<UserAchievementDto>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    _logger.LogInformation("FuelConversion configuration error");
                    return Result<UserAchievementDto>.Failure("FuelConversion configuration error.");
                }
                catch (Exception ex) when (ex is OverflowException)
                {
                    _logger.LogInformation("FuelConversion configuration error");
                    return Result<UserAchievementDto>.Failure("FuelConversion configuration error.");
                }
            }
        }
    }
}