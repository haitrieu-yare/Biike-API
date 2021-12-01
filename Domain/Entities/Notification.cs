using System;

namespace Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NotificationType { get; set; }
        public int UserType { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;
        public string CoverImage { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}