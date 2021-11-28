using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoTripCancellation : IJob
    {
        private readonly DataContext _context;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<AutoTripCancellation> _logger;

        public AutoTripCancellation(DataContext context, ISchedulerFactory schedulerFactory, 
            ILogger<AutoTripCancellation> logger)
        {
            _context = context;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                var tripId = Convert.ToInt32(dataMap.GetString("TripId"));

                if (tripId == 0)
                {
                    _logger.LogError("Could not find tripId for trip cancellation");
                    return;
                }

                Trip trip = await _context.Trip.FindAsync(tripId);

                if (trip == null)
                {
                    _logger.LogError("Trip with {TripId} doesn't exist", tripId);
                    return;
                }
            
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                string jobName = Constant.GetJobNameAutoCancellation(trip.TripId);

                switch (trip.Status)
                {
                    case (int) TripStatus.Finished:
                        _logger.LogInformation("Trip has already finished");
                        return;
                    case (int) TripStatus.Cancelled:
                        _logger.LogInformation("Trip has already cancelled");
                        return;
                    case (int) TripStatus.Finding:
                        trip.CancelReason = "Tự động hủy vì đã quá giờ khởi hành.";
                        trip.Status = (int) TripStatus.Cancelled;
                        trip.CancelTime = CurrentTime.GetCurrentTime();
                        await scheduler.DeleteJob(JobKey.Create(jobName, Constant.OneTimeJob), CancellationToken.None);
                        break;
                    default:
                        trip.CancelReason = "Tự động hủy vì đã quá ngày khởi hành.";
                        trip.Status = (int) TripStatus.Cancelled;
                        trip.CancelTime = CurrentTime.GetCurrentTime();
                        await scheduler.DeleteJob(JobKey.Create(jobName, Constant.OneTimeJob), CancellationToken.None);
                        break;
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result)
                {
                    _logger.LogError("Failed to automatically cancel trip with TripId {TripId}", tripId);
                    return;
                }

                _logger.LogInformation("Successfully automatically cancelled trip with TripId {TripId}", tripId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }
    }
}