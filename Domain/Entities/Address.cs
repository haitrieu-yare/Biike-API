﻿using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressName { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        
        public AdvertisingAddress? AdvertisingAddress { get; set; }
        public UserAddress? UserAddress { get; set; }
    }
}