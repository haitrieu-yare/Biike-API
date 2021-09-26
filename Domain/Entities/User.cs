using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string PhoneNumber { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Avatar { get; set; } = string.Empty;
		public int Gender { get; set; } = (int)GenderStatus.Male;
		public int Status { get; set; } = (int)UserStatus.Deactive;
		public string? LastLoginDevice { get; set; }
		public DateTime? LastTimeLogin { get; set; }
		public double Star { get; set; } = 4;
		public int TotalPoint { get; set; } = 0;
		public DateTime? BirthDate { get; set; }
		public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
		public bool IsBikeVerified { get; set; } = false;

		[InverseProperty("Keer")]
		public ICollection<Trip> KeerTrips { get; set; } = new List<Trip>();

		[InverseProperty("Biker")]
		public ICollection<Trip> BikerTrips { get; set; } = new List<Trip>();

		[InverseProperty("UserOne")]
		public ICollection<Intimacy> UserOneIntimacies { get; set; } = new List<Intimacy>();

		[InverseProperty("UserTwo")]
		public ICollection<Intimacy> UserTwoIntimacies { get; set; } = new List<Intimacy>();
		public ICollection<Feedback> FeedBackList { get; set; } = new List<Feedback>();
		public ICollection<Bike> Bikes { get; set; } = new List<Bike>();
		public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
	}
}