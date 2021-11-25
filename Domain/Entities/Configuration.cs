using System;

namespace Domain.Entities
{
    public class Configuration
    {
        public int ConfigurationId { get; set; }
        public string ConfigurationName { get; set; } = string.Empty;
        public string ConfigurationValue { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}