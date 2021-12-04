using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Notifications;
using Application.Notifications.DTOs;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Trips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoNotificationSending : IJob
    {
        private readonly ILogger<AutoNotificationSending> _logger;
        private readonly IConfiguration _configuration;
        private readonly NotificationSending _notiSender;

        public AutoNotificationSending(ILogger<AutoNotificationSending> logger, IConfiguration configuration,
            NotificationSending notiSender)
        {
            _logger = logger;
            _configuration = configuration;
            _notiSender = notiSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                List<int> bikerIds = new();
                for (var i = 0; i < 3; i++)
                {
                    var bikerId = Convert.ToInt32(dataMap.GetString($"BikerId{i + 1}"));
                    if (bikerId != 0)
                    {
                        bikerIds.Add(bikerId);
                    }
                }
                var tripId = dataMap.GetIntValue("TripId");

                if (bikerIds.Count == 0)
                {
                    _logger.LogInformation("Could not find BikerId for trip notification");
                    return;
                }

                foreach (var bikerId in bikerIds)
                {
                    // ReSharper disable StringLiteralTypo
                    var notification = new NotificationDto
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = Constant.NotificationTitleKeNow,
                        Content = Constant.NotificationContentKeNow,
                        ReceiverId = bikerId,
                        Url = $"{_configuration["ApiPath"]}/trips/{tripId}/details",
                        IsRead = false,
                        CreatedDate = CurrentTime.GetCurrentTime()
                    };
                    // ReSharper restore StringLiteralTypo

                    await _notiSender.Run(notification);
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