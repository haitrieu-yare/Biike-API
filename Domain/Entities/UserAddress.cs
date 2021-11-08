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
        public string AddressName { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public string AddressCoordinate { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}