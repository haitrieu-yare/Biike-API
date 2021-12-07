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

            List<ITrigger> triggers = new();

            string triggerNameFinding = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Finding");
            string triggerNameMatched = Constant.GetTriggerNameAutoCancellation(trip.TripId, "Matched");

            var bookTimeInVietNam = trip.BookTime;
            var bookTimeNextDayInVietNam = bookTimeInVietNam.AddDays(1);
            // For example bookTimeInVietNam is 2021-12-02 15:00:00 in VietName Time
            // bookTimeNextDayInVietNam will be 2021-12-03 15:00:00 in VietName Time
            // New DateTime will get the time of the local time
            // If this code run on Azure server which uses UTC
            // It will get the new time in UTC, so the result of bookTimeNextDayAt12AmInUtc
            // will be 2021-12-03 00:00:00 in UTC, but this is not what we want
            // we want the trigger to trigger at 12AM in VietName time
            // Not at 12AM in UTC time, so we have to minus 7 hours at line 56,
            // and the time will be 2021-12-02 17:00:00 in UTC
            var bookTimeNextDayAt12AmInUtc = new DateTime(bookTimeNextDayInVietNam.Year, bookTimeNextDayInVietNam.Month,
                bookTimeNextDayInVietNam.Day, 0, 0, 0);

            var findingTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                // Although bookTimeInVietNam is in VietNam time
                // The code itself, if run on Azure server, which uses UTC time
                // will think this time is in UTC, not in VietName time
                // So we have to minus 7 hours to make it actually run in VietNam time
                .StartAt(trip.IsScheduled
                    ? CurrentTime.ToLocalTime(bookTimeInVietNam)
                    : CurrentTime.ToLocalTime(bookTimeInVietNam.AddMinutes(-5)))
                .Build();
            var matchedTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameMatched, Constant.OneTimeJob)
                .StartAt(CurrentTime.ToLocalTime(bookTimeNextDayAt12AmInUtc))
                .Build();

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

            await scheduler.ScheduleJob(job, triggers, true);
        }
    }
}