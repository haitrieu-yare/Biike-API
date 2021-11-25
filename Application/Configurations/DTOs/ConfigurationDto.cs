using System;

namespace Application.Configurations.DTOs
{
    public class ConfigurationDto
    {
        public int? ConfigurationId { get; set; }
        public string? ConfigurationName { get; set; }
        public string? ConfigurationValue { get; set; } 
        public int? UserId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}