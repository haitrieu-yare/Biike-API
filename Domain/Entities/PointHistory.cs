using System;

namespace Domain.Entities
{
    public class PointHistory
    {
        public int PointHistoryId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int HistoryType { get; set; }
        public int RelatedId { get; set; }
        public int Point { get; set; }
        public int TotalPoint { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
    }
}