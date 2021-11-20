using System;

namespace Application.Reports.DTOs
{
    public class ReportDto
    {
        public int? ReportId { get; set; }
        public int? UserOneId { get; set; }
        public string? UserOneName { get; set; }
        public int? UserTwoId { get; set; }
        public string? UserTwoName { get; set; } 
        public string? ReportReason { get; set; }
        public int? ReportStatus { get; set; }
        public DateTime? CreatedDate { get; set; } 
    }
}