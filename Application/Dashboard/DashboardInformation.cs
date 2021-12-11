using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dashboard.DTOs;
using Domain;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Dashboard
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DashboardInformation
    {
        public class Query : IRequest<Result<DashboardDto>>
        {
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<DashboardDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<DashboardDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentTime = CurrentTime.GetCurrentTime();
                    
                    var totalKmSaved = await _context.Trip
                        .Include(t => t.Route)
                        .Where(t => t.Status == (int) TripStatus.Finished)
                        .Select(t => t.Route.Distance)
                        .SumAsync(cancellationToken);

                    var fuelConversion = await _context.Configuration
                        .Where(c => c.ConfigurationName.Equals("FuelConversion"))
                        .SingleOrDefaultAsync(cancellationToken);

                    if (fuelConversion == null)
                    {
                        _logger.LogInformation("Can not find fuel conversion configuration");
                        return Result<DashboardDto>.Failure("Can not find fuel conversion configuration.");
                    }
                    
                    var totalFuelSaved = totalKmSaved * Convert.ToDouble(fuelConversion.ConfigurationValue);

                    var totalUser = await _context.User.CountAsync(cancellationToken);
                    var totalNewUser = await _context.User
                        .Where(u => u.CreatedDate.CompareTo(currentTime.AddDays(-30)) > 0)
                        .CountAsync(cancellationToken);
                    
                    var totalTrip = await _context.Trip.CountAsync(cancellationToken);

                    var totalRedemption = await _context.Redemption.CountAsync(cancellationToken);
                    var totalPointUsedForVoucher = await _context.Redemption
                        .Select(r => r.VoucherPoint)
                        .SumAsync(cancellationToken);
                    
                    var totalAdsClickCount = await _context.Advertisement
                        .Select(a => a.TotalClickCount)
                        .SumAsync(cancellationToken);

                    List<TripStatusPercentageDto> tripStatusPercentage = new();

                    int findingTrip = 0;
                    int matchedTrip = 0;
                    int waitingTrip = 0;
                    int startedTrip = 0;
                    int finishedTrip = 0;
                    // int cancelledTrip = 0;

                    if (totalTrip != 0)
                    {
                        findingTrip = await _context.Trip
                            .Where(t => t.Status == (int) TripStatus.Finding)
                            .CountAsync(cancellationToken);
                    
                        matchedTrip = await _context.Trip
                            .Where(t => t.Status == (int) TripStatus.Matched)
                            .CountAsync(cancellationToken);
                    
                        waitingTrip = await _context.Trip
                            .Where(t => t.Status == (int) TripStatus.Waiting)
                            .CountAsync(cancellationToken);
                    
                        startedTrip = await _context.Trip
                            .Where(t => t.Status == (int) TripStatus.Started)
                            .CountAsync(cancellationToken);
                    
                        finishedTrip = await _context.Trip
                            .Where(t => t.Status == (int) TripStatus.Finished)
                            .CountAsync(cancellationToken);
                    
                        // cancelledTrip = await _context.Trip
                        //     .Where(t => t.Status == (int) TripStatus.Cancelled)
                        //     .CountAsync(cancellationToken);
                    }

                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Finding,
                        Percentage = totalTrip == 0 ? 0 :
                            ApplicationUtils.ToPercentage(findingTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Matched,
                        Percentage = totalTrip == 0 ? 0 :
                            ApplicationUtils.ToPercentage(matchedTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Waiting,
                        Percentage = totalTrip == 0 ? 0 :
                            ApplicationUtils.ToPercentage(waitingTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Started,
                        Percentage = totalTrip == 0 ? 0 :
                            ApplicationUtils.ToPercentage(startedTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Finished,
                        Percentage = totalTrip == 0 ? 0 :
                            ApplicationUtils.ToPercentage(finishedTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    var percentage = tripStatusPercentage.Select(t => t.Percentage).Sum();
                    
                    tripStatusPercentage.Add(new TripStatusPercentageDto()
                    {
                        TripStatus = (int) TripStatus.Cancelled,
                        Percentage = totalTrip == 0 ? 0 : Math.Round(100 - percentage, 2, MidpointRounding.AwayFromZero)
                            // ApplicationUtils.ToPercentage(cancelledTrip / Convert.ToDouble(totalTrip))
                    });
                    
                    List<StationPercentageDto> stationPercentage = new();
                    List<StationCountDto> stationCount = new();

                    var departureStationOccurence = await _context.Trip
                        .GroupBy(t => t.Route.DepartureId)
                        .Select(t => new StationCountDto
                        {
                            StationId = t.Key,
                            Count = t.Count()
                        })
                        .ToListAsync(cancellationToken);
                    
                    var destinationStationOccurence = await _context.Trip
                        .GroupBy(t => t.Route.DestinationId)
                        .Select(t => new StationCountDto
                        {
                            StationId = t.Key,
                            Count = t.Count()
                        })
                        .ToListAsync(cancellationToken);
                    
                    stationCount.AddRange(departureStationOccurence);

                    foreach (var destinationStation in destinationStationOccurence)
                    {
                        var station = stationCount
                            .Find(t => t.StationId == destinationStation.StationId);
                        
                        if (station != null)
                        {
                            station.Count += destinationStation.Count;
                        }
                        else
                        {
                            stationCount.Add(destinationStation);
                        }
                    }

                    int totalStationUsedInTrip;

                    if (stationCount.Count > 0)
                    {
                        totalStationUsedInTrip = stationCount.Select(s => s.Count).Sum();

                        foreach (var station in stationCount)
                        {
                            var stationName = await _context.Station
                                .Where(s => s.StationId == station.StationId)
                                .Select(s => s.Name)
                                .SingleOrDefaultAsync(cancellationToken);
                            
                            station.StationName = stationName;
                        }

                        stationPercentage.AddRange(
                            stationCount.Select(station => new StationPercentageDto
                            {
                                StationId = station.StationId, 
                                StationName = station.StationName, 
                                Percentage = ApplicationUtils.ToPercentage(
                                    station.Count / Convert.ToDouble(totalStationUsedInTrip))
                            }));
                    }

                    var userAchievementDto = new DashboardDto
                    {
                        TotalUser = totalUser,
                        TotalNewUser = totalNewUser,
                        TotalTrip = totalTrip,
                        TotalRedemption = totalRedemption,
                        TotalPointUsedForVoucher = totalPointUsedForVoucher,
                        TotalAdsClickCount = totalAdsClickCount,
                        TotalKmSaved = Math.Round(totalKmSaved, 2, MidpointRounding.AwayFromZero),
                        TotalFuelSaved = Math.Round(totalFuelSaved, 2, MidpointRounding.AwayFromZero),
                        StationPercentage = stationPercentage,
                        TripStatusPercentage = tripStatusPercentage
                    };

                    _logger.LogInformation("Successfully retrieved user self profile");
                    return Result<DashboardDto>.Success(
                        userAchievementDto, "Successfully retrieved user self profile.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<DashboardDto>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    _logger.LogInformation("FuelConversion configuration error");
                    return Result<DashboardDto>.Failure("FuelConversion configuration error.");
                }
                catch (Exception ex) when (ex is OverflowException)
                {
                    _logger.LogInformation("FuelConversion configuration error");
                    return Result<DashboardDto>.Failure("FuelConversion configuration error.");
                }
            }
        }
    }
}