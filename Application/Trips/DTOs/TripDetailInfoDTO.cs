using System;
using System.Collections.Generic;
using Application.Feedbacks.DTOs;

namespace Application.Trips.DTOs
{
	public class TripDetailInfoDto
	{
		public int? TripId { get; set; }
		public int? UserId { get; set; }
		public string? UserPhoneNumber { get; set; }
		public string? UserFullname { get; set; }
		public double? UserStar { get; set; }
		public string? Avatar { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? TimeBook { get; set; }
		public DateTime? TimeFinished { get; set; }
		public int? TripStatus { get; set; }
		public string? StartingPointName { get; set; }
		public string? DestinationName { get; set; }
		public List<FeedbackDto> Feedbacks { get; set; } = new();
	}
}