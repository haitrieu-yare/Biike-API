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
            string triggerNameMatched = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Matched");

            var bookTime = CurrentTime.ToLocalTime(trip.BookTime);
            var bookTimeNextDay = bookTime.AddDays(1);
            var bookTimeNextDayAt12Am = new DateTime(bookTimeNextDay.Year, bookTimeNextDay.Month, 
                bookTimeNextDay.Day, 0, 0, 0);

            var findingTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                .StartAt(bookTime)
                .Build();
            var matchedTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameMatched, Constant.OneTimeJob)
                .StartAt(bookTimeNextDayAt12Am)
                .Build();

            List<ITrigger> triggers = new();

            switch (trip.Status)
            {
                case (int) TripStatus.Finding:
                    triggers.Add(findingTrigger);
                    triggers.Add(matchedTrigger);
                    break;
                case (int) TripStatus.Matched:
                    triggers.Add(matchedTrigger);
                    break;
                case (int) TripStatus.Waiting:
                    triggers.Add(matchedTrigger);
                    break;
                case (int) TripStatus.Started:
                    triggers.Add(matchedTrigger);
                    break;
            }

            await scheduler.ScheduleJob(job, triggers,true);
        }
    }
}