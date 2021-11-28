using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoTripCancellationStartup : IJob
    {
        private readonly DataContext _context;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<AutoTripCancellationStartup> _logger;

        public AutoTripCancellationStartup(DataContext context, ISchedulerFactory schedulerFactory, 
            ILogger<AutoTripCancellationStartup> logger)
        {
            _context = context;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<Trip> trips = await _context.Trip
                    .Where(t => t.Status == (int) TripStatus.Finding || 
                                    t.Status == (int) TripStatus.Matching ||
                                    t.Status == (int) TripStatus.Waiting ||
                                    t.Status == (int) TripStatus.Started).ToListAsync();

                foreach (var trip in trips) await AutoTripCancellationCreation.Run(_schedulerFactory, trip);
            }
            catch (Exception e)
            {
                _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }
    }
}