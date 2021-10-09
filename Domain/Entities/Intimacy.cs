using System;

namespace Domain.Entities
{
    public class Intimacy
    {
        public int UserOneId { get; set; }
        public User UserOne { get; set; } = null!;
        public int UserTwoId { get; set; }
        public User UserTwo { get; set; } = null!;
        public bool IsBlock { get; set; } = true;
        public DateTime BlockTime { get; set; } = CurrentTime.GetCurrentTime();
        public DateTime? UnblockTime { get; set; }
    }
}