using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Notifications.DTOs;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Notifications
{
    public class NotificationSending
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationSending> _logger;

        public NotificationSending(DataContext context, IConfiguration configuration,
            ILogger<NotificationSending> logger)
        {
            _context = context;
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
                        AuthTokenAsyncFactory = () =>
                            Task.FromResult(_configuration["Firebase:RealtimeDatabaseSecret"])
                    });

                var options = new JsonSerializerOptions {WriteIndented = true};

                string notificationJsonString = JsonSerializer.Serialize(notification, options);

                await firebaseClient.Child("notification")
                    .Child($"{notification.ReceiverId}")
                    .PostAsync(notificationJsonString);

                _logger.LogInformation("Successfully sent notification to {ReceiverId}", notification.ReceiverId);

                const string requestUrl = "https://fcm.googleapis.com/fcm/send";

                var user = await _context.User.FindAsync(notification.ReceiverId!);

                if (user == null || user.IsDeleted)
                {
                    _logger.LogError("Can't find user with UserId {request.UserId} " + "to send push notification",
                        notification.ReceiverId);
                    return;
                }

                if (!string.IsNullOrEmpty(user.LastLoginDevice))
                {
                    try
                    {
                        var pushNotification = new PushNotificationDto
                        {
                            To = user.LastLoginDevice,
                            Notification = new PushNotificationBodyDto
                            {
                                Body = notification.Content,
                                // ReSharper disable once RedundantAnonymousTypePropertyName
                                Title = notification.Title,
                            },
                        };

                        string requestBody = JsonSerializer.Serialize(pushNotification, options);
                        HttpContent stringContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        HttpClient client = new();
                        string authorizationValue = $"={_configuration["Firebase:CloudMessagingSecret"]}";
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("key", authorizationValue);

                        HttpResponseMessage task =
                            await client.PostAsync(requestUrl, stringContent, CancellationToken.None);

                        if (!task.IsSuccessStatusCode)
                        {
                            _logger.LogError("Failed to send push notification");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("{Error}", e.InnerException?.Message ?? e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("{Error}", e.InnerException?.Message ?? e.Message);
                throw;
            }
        }
    }
}