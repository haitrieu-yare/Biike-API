using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoTripCancellationStartup : IJob
    {
        private readonly DataContext _context;
        private readonly ISchedulerFactory _schedulerFactory;

        public AutoTripCancellationStartup(DataContext context, ISchedulerFactory schedulerFactory)
        {
            _context = context;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<Domain.Entities.Trip> trips = await _context.Trip.Where(t => t.Status == (int) TripStatus.Finding).ToListAsync();

            foreach (var trip in trips) await AutoTripCancellationCreation.Run(_schedulerFactory, trip);
        }
    }
}