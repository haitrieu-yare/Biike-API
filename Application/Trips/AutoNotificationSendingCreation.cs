using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Quartz;

namespace Application.Trips
{
    public static class AutoNotificationSendingCreation
    {
        public static async Task Run(ISchedulerFactory? schedulerFactory, Trip? trip, List<int> bikerIds)
        {
            if (trip == null || schedulerFactory == null) return;

            IScheduler scheduler = await schedulerFactory.GetScheduler();

            List<int> remainingBikerIdsPartOne = bikerIds.Take(3).ToList();
            List<int> remainingBikerIdsPartTwo = bikerIds.Skip(3).Take(3).ToList();

            #region PartOne

            if (remainingBikerIdsPartOne.Count == 0)
            {
                return;
            }
            
            // Mobile sẽ gửi time cộng thêm sẵn 15 phút, nên phải trừ 15 phút đi để gửi notification
            var bookTimeInVietNam = trip.BookTime.AddMinutes(-15);
            var bookTimeNext3MinInVietNam = bookTimeInVietNam.AddMinutes(3);

            JobDataMap jobDataMapPartOne = new();
            foreach (var bikerId in remainingBikerIdsPartOne)
            {
                jobDataMapPartOne.Add($"BikerId{jobDataMapPartOne.Count + 1}", bikerId.ToString());
            }
            
            string jobNamePartOne = Constant.GetJobNameAutoNotificationSending(trip.TripId, "Part One");

            IJobDetail jobOne = JobBuilder.Create<AutoNotificationSending>()
                .WithIdentity(jobNamePartOne, Constant.OneTimeJob)
                .UsingJobData(jobDataMapPartOne)
                .UsingJobData("TripId", $"{trip.TripId}")
                .Build();
            
            string triggerNamePartOne = Constant.GetTriggerNameNotificationSending(trip.TripId, "PartOne");
            
            var partOneTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNamePartOne, Constant.OneTimeJob)
                // Although bookTimeNext3MinInVietNam is in VietNam time
                // The code itself, if run on Azure server, which uses UTC time,
                // The code will think this time is in UTC, not in VietName time
                // So we have to minus 7 hours to make it actually run in VietNam time
                // .StartAt(bookTimeNext3MinInVietNam.AddHours(-7))
                .StartAt(bookTimeNext3MinInVietNam)
                .Build();
            
            await scheduler.ScheduleJob(jobOne, partOneTrigger);

            #endregion

            #region PartTwo

            if (remainingBikerIdsPartTwo.Count == 0)
            {
                return;
            }
            
            var bookTimeNext6MinInVietNam = bookTimeInVietNam.AddMinutes(6);
            
            JobDataMap jobDataMapPartTwo = new();
            foreach (var bikerId in remainingBikerIdsPartTwo)
            {
                jobDataMapPartTwo.Add($"BikerId{jobDataMapPartTwo.Count + 1}", bikerId.ToString());
            }
            
            string jobNamePartTwo = Constant.GetJobNameAutoNotificationSending(trip.TripId, "Part Two");

            IJobDetail jobTwo = JobBuilder.Create<AutoNotificationSending>()
                .WithIdentity(jobNamePartTwo, Constant.OneTimeJob)
                .UsingJobData(jobDataMapPartTwo)
                .UsingJobData("TripId", $"{trip.TripId}")
                .Build();

            string triggerNamePartTwo = Constant.GetTriggerNameNotificationSending(trip.TripId, "PartTwo");

            var partTwoTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerNamePartTwo, Constant.OneTimeJob)
                // Although bookTimeNext3MinInVietNam is in VietNam time
                // The code itself, if run on Azure server, which uses UTC time,
                // The code will think this time is in UTC, not in VietName time
                // So we have to minus 7 hours to make it actually run in VietNam time
                // .StartAt(bookTimeNext6MinInVietNam.AddHours(-7))
                .StartAt(bookTimeNext6MinInVietNam)
                .Build();

            await scheduler.ScheduleJob(jobTwo, partTwoTrigger);

            #endregion
        }
    }
}