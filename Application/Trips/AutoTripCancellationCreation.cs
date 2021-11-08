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
            try
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
                var bookTimeNextDay = bookTime.AddDays(1);
                var bookTimeNextDayAt12Am = new DateTime(bookTimeNextDay.Year, bookTimeNextDay.Month, 
                    bookTimeNextDay.Day, 0, 0, 0);

                var findingTrigger = TriggerBuilder.Create()
                    .WithIdentity(triggerNameFinding, Constant.OneTimeJob)
                    .StartAt(bookTime)
                    .Build();
                var waitingTrigger = TriggerBuilder.Create()
                    .WithIdentity(triggerNameWaiting, Constant.OneTimeJob)
                    .StartAt(bookTimeNextDayAt12Am)
                    .Build();

                List<ITrigger> triggers = new();

                switch (trip.Status)
                {
                    case (int) TripStatus.Finding:
                        triggers.Add(findingTrigger);
                        triggers.Add(waitingTrigger);
                        break;
                    case (int) TripStatus.Waiting:
                        triggers.Add(waitingTrigger);
                        break;
                }

                await scheduler.ScheduleJob(job, triggers,true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}