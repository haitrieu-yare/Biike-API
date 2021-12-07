using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Notifications.DTOs
{
    public class NotificationDto
    {
        [JsonPropertyName("notificationId")] public Guid? NotificationId { get; set; }
        [Required] [JsonPropertyName("receiverId")] public int? ReceiverId { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("content")] public string? Content { get; set; }
        [JsonPropertyName("url")] public string? Url { get; set; }
        [JsonPropertyName("isRead")] public bool? IsRead { get; set; }
        [JsonPropertyName("createdDate")] public DateTime? CreatedDate { get; set; }
    }
}