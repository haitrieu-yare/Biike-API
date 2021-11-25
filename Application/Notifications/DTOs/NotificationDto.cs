using System;

namespace Application.Notifications.DTOs
{
    public class NotificationDto
    {
        public int? NotificationId { get; set; }
        public int? ReceiverId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}