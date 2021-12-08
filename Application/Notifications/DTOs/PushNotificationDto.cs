using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Notifications.DTOs
{
    public class PushNotificationDto
    {
        [JsonPropertyName("to")] [Required] public string? To { get; set; }

        [JsonPropertyName("notification")]
        [Required]
        public PushNotificationBodyDto? Notification { get; set; }
    }
}