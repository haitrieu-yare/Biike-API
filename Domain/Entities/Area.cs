using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Domain.Entities
{
    public class Area
    {
        public int AreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public bool IsDeleted { get; set; }
        public ICollection<Station> Stations { get; set; } = new List<Station>();
        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}