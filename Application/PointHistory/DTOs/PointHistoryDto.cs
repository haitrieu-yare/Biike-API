using System;

namespace Application.PointHistory.DTOs
{
    public class PointHistoryDto
    {
        public int? PointHistoryId { get; set; }
        public int? UserId { get; set; }
        public int? HistoryType { get; set; }
        public int? RelatedId { get; set; }
        public int? Point { get; set; }
        public int? TotalPoint { get; set; }
        public string? Description { get; set; } 
        public DateTime? TimeStamp { get; set; }
    }
}