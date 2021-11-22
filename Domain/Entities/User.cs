using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

// ReSharper disable CollectionNeverUpdated.Global

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress] 
        [RegularExpression(@".*@fpt\.edu\.vn$", ErrorMessage = "Must use fpt email.")]
        public string Email { get; set; } = "thisisadefaultmail@fpt.edu.vn";
        public string PasswordHash { get; set; } = "092021";
        public int Role { get; set; } = (int) RoleStatus.Keer;
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Gender { get; set; } = (int) GenderStatus.Other;
        public int Status { get; set; } = (int) UserStatus.Deactive;
        public string? LastLoginDevice { get; set; }
        public DateTime? LastTimeLogin { get; set; }
        public double Star { get; set; } = 5;
        public int TotalPoint { get; set; }
        public int MaxTotalPoint { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsBikeVerified { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
        public bool IsDeleted { get; set; }
        [InverseProperty("Keer")] public ICollection<Trip> KeerTrips { get; set; } = new List<Trip>();
        [InverseProperty("Biker")] public ICollection<Trip> BikerTrips { get; set; } = new List<Trip>();

        [InverseProperty("UserOne")]
        public ICollection<Intimacy> UserOneIntimacies { get; set; } = new List<Intimacy>();

        [InverseProperty("UserTwo")]
        public ICollection<Intimacy> UserTwoIntimacies { get; set; } = new List<Intimacy>();
        
        [InverseProperty("UserOne")]
        public ICollection<Report> UserOneReports { get; set; } = new List<Report>();

        [InverseProperty("UserTwo")]
        public ICollection<Report> UserTwoReports { get; set; } = new List<Report>();

        public ICollection<Feedback> FeedBackList { get; set; } = new List<Feedback>();
        public ICollection<Bike> Bikes { get; set; } = new List<Bike>();
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public ICollection<PointHistory> PointHistories { get; set; } = new List<PointHistory>();
    }
}