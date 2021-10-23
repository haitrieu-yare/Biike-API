using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Quartz;

namespace Application.Trips
{
    public static class AutoTripCancellationCreation
    {
        public static async Task Run(ISchedulerFactory? schedulerFactory, Trip? trip)
        {
            if (trip == null || schedulerFactory == null) return;

            IScheduler scheduler = await schedulerFactory.GetScheduler();

            string jobName = Constant.GetJobNameAutoCancellation(trip.TripId);

            IJobDetail job = JobBuilder.Create<AutoTripCancellation>()
                .WithIdentity(jobName, Constant.OneTimeJob)
                .UsingJobData("TripId", $"{trip.TripId}")
                .Build();

            string triggerName = Constant.GetTriggerNameAutoCancellation(trip.TripId);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(triggerName, Constant.OneTimeJob)
                .StartAt(CurrentTime.ToLocalTime(trip.BookTime))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}