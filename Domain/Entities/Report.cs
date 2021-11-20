using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Report
    {
        public int ReportId { get; set; }
        public int UserOneId { get; set; }
        public User UserOne { get; set; } = null!;
        public string UserOneName { get; set; } = string.Empty;
        public int UserTwoId { get; set; }
        public User UserTwo { get; set; } = null!;
        public string UserTwoName { get; set; } = string.Empty;
        public string ReportReason { get; set; } = string.Empty;
        public int Status { get; set; } = (int) ReportStatus.New;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}