using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;
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

            string triggerNameFinding = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Finding");
            string triggerNameWaiting = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Waiting");

            var bookTime = CurrentTime.ToLocalTime(trip.BookTime);
            var bookTimeNextDay = new DateTime(bookTime.Year, bookTime.Month, bookTime.Day + 1, 0, 0, 0);

            IReadOnlyCollection<ITrigger> triggers = new List<ITrigger>
            {
                TriggerBuilder.Create()
                    .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                    .StartAt(bookTime)
                    .Build(),
                TriggerBuilder.Create()
                    .WithIdentity(triggerNameWaiting, Constant.OneTimeJob)
                    .StartAt(bookTimeNextDay)
                    .Build()
            };

            await scheduler.ScheduleJob(job, triggers,true);
        }
    }
}