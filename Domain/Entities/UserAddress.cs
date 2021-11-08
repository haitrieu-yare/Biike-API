// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

using System;

namespace Domain.Entities
{
    public class UserAddress
    {
        public int UserAddressId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string UserAddressName { get; set; } = string.Empty;
        public string UserAddressDetail { get; set; } = string.Empty;
        public string UserAddressCoordinate { get; set; } = string.Empty;
        public string UserAddressNote { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}