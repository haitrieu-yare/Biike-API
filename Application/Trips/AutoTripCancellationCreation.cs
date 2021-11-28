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
            string triggerNameMatching = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Matching");

            var bookTime = CurrentTime.ToLocalTime(trip.BookTime);
            var bookTimeNextDay = bookTime.AddDays(1);
            var bookTimeNextDayAt12Am = new DateTime(bookTimeNextDay.Year, bookTimeNextDay.Month, 
                bookTimeNextDay.Day, 0, 0, 0);

            var findingTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                .StartAt(bookTime)
                .Build();
            var matchingTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameMatching, Constant.OneTimeJob)
                .StartAt(bookTimeNextDayAt12Am)
                .Build();

            List<ITrigger> triggers = new();

            switch (trip.Status)
            {
                case (int) TripStatus.Finding:
                    triggers.Add(findingTrigger);
                    triggers.Add(matchingTrigger);
                    break;
                case (int) TripStatus.Matching:
                    triggers.Add(matchingTrigger);
                    break;
                case (int) TripStatus.Waiting:
                    triggers.Add(matchingTrigger);
                    break;
                case (int) TripStatus.Started:
                    triggers.Add(matchingTrigger);
                    break;
            }

            await scheduler.ScheduleJob(job, triggers,true);
        }
    }
}