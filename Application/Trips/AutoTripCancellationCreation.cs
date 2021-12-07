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
            var midnightInVietNam = CurrentTime.ToLocalTime(new DateTime(bookTimeInVietNam.Year,
                bookTimeInVietNam.Month, bookTimeInVietNam.Day, 17, 0, 0, DateTimeKind.Utc));

            var findingTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                // ToCorrespondingUtcTime() will change VietNam time to corresponding time in UTC
                .StartAt(trip.IsScheduled
                    ? CurrentTime.ToUtcTime(bookTimeInVietNam)
                    : CurrentTime.ToUtcTime(bookTimeInVietNam.AddMinutes(-5)))
                .Build();
            var matchedTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNameMatched, Constant.OneTimeJob)
                .StartAt(midnightInVietNam)
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