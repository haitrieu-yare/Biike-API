using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Notifications.DTOs
{
    public class PushNotificationBodyDto
    {
        [JsonPropertyName("sound")] public string Sound { get; set; } = "default";
        [JsonPropertyName("body")] [Required] public string? Body { get; set; }
        [JsonPropertyName("title")] [Required] public string? Title { get; set; }

        [JsonPropertyName("content_available")]
        public bool ContentAvailable { get; set; } = true;

        [JsonPropertyName("priority")] public string Priority { get; set; } = "high";
    }
}