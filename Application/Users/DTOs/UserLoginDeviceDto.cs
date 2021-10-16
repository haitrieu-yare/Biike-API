using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Users.DTOs
{
    public class UserLoginDeviceDto
    {
        [Required] public string? LastLoginDevice { get; init; }
        
        public DateTime? LastTimeLogin { get; set; }
    }
}