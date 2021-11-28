using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Notifications.DTOs;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Notifications
{
    public class NotificationSending
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationSending> _logger;

        public NotificationSending(IConfiguration configuration, ILogger<NotificationSending> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Run(NotificationDto notification)
        {
            try
            {
                var firebaseClient = new FirebaseClient(_configuration["Firebase:RealtimeDatabase"],
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(_configuration["Firebase:RealtimeDatabaseSecret"])
                    });

                var options = new JsonSerializerOptions {WriteIndented = true};

                string notificationJsonString = JsonSerializer.Serialize(notification, options);

                await firebaseClient.Child("notification")
                    .Child($"{notification.ReceiverId}")
                    .PostAsync(notificationJsonString);
            
                _logger.LogInformation("Successfully sent notification to {ReceiverId}", notification.ReceiverId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("{Error}", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }
    }
}