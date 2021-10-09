using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public bool IsDeleted { get; set; } = false;
        public ICollection<Station> Stations { get; set; } = new List<Station>();
    }
}