using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
	[Index(nameof(PhoneNumber), IsUnique = true)]
	[Index(nameof(Email), IsUnique = true)]
	public class AppUser
	{
		public int Id { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Email { get; set; }
		public string FullName { get; set; }
		public string Avatar { get; set; }
		public int Gender { get; set; }
		public int Status { get; set; }
		public string LastLoginDevice { get; set; }
		public DateTime? LastTimeLogin { get; set; }
		public double Star { get; set; }
		public DateTime? BirthDate { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsBikeVerified { get; set; }

		[InverseProperty("Keer")]
		public ICollection<Trip> KeerTrips { get; set; }

		[InverseProperty("Biker")]
		public ICollection<Trip> BikerTrips { get; set; }

		[InverseProperty("UserOne")]
		public ICollection<Intimacy> UserOneIntimacies { get; set; }

		[InverseProperty("UserTwo")]
		public ICollection<Intimacy> UserTwoIntimacies { get; set; }
		public ICollection<Feedback> FeedBackList { get; set; }
		public ICollection<Bike> Bikes { get; set; }
		public Wallet Wallet { get; set; }
	}
}