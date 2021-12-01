using System;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities
{
    public class Bike
    {
        public int BikeId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string BikeOwner { get; set; } = string.Empty;
        public string BikePicture { get; set; } = string.Empty;
        public string BikeLicensePicture { get; set; } = string.Empty;
        public string PlateNumberPicture { get; set; } = string.Empty;
        public string DrivingLicenseFrontPicture { get; set; } = string.Empty;
        public string DrivingLicenseBackPicture { get; set; } = string.Empty;

        public int Status { get; set; } = (int) BikeStatus.Unverified;
        public string? FailedVerificationReason { get; set; }
        [MinLength(4)] public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string BikeType { get; set; } = string.Empty;
        public string BikeVolume { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}