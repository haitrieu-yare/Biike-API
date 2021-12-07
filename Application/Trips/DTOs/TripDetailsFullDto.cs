using System;
using System.Collections.Generic;
using Application.Feedbacks.DTOs;

namespace Application.Trips.DTOs
{
    public class TripDetailsFullDto
    {
        public int? TripId { get; set; }
        public int? UserId { get; set; }
        public int? KeerId { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserFullname { get; set; }
        public double? UserStar { get; set; }
        public string? Avatar { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? NumberPlate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? BookTime{ get; set; }
        public DateTime? FirstPersonArrivalTime { get; set; }
        public int? FirstPersonArrivalId { get; set; }
        public DateTime? SecondPersonArrivalTime { get; set; }
        public int? SecondPersonArrivalId { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string? CancelPersonFullname { get; set; }
        public string? CancelReason { get; set; }
        public int? TripStatus { get; set; }
        public string? DepartureName { get; set; }
        public string? DepartureCoordinate { get; set; }
        public string? DestinationName { get; set; }
        public string? DestinationCoordinate { get; set; }
        public List<FeedbackDto> Feedbacks { get; set; } = new();
    }
}