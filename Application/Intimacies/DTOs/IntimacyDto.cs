using System;

namespace Application.Intimacies.DTOs
{
    public class IntimacyDto
    {
        public int? UserOneId { get; set; }
        public int? UserTwoId { get; set; }
        public string? UserName { get; set; }
        public bool? IsBlock { get; set; }
        public DateTime? BlockTime { get; set; }
        public DateTime? UnBlockTime { get; set; }
    }
}