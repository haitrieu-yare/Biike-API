using System;

namespace Application.Trips.DTOs
{
    public class TripDetailsDto
    {
        public int? TripId { get; set; }
        public int? KeerId { get; set; }
        public string? KeerFullname { get; set; }
        public int? BikerId { get; set; }
        public string? BikerFullname { get; set; }
        public int? RouteId { get; set; }
        public string? DepartureStationName { get; set; }
        public string? DestinationStationName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime BookTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? FirstPersonArrivalTime { get; set; }
        public int? FirstPersonArrivalId { get; set; }
        public DateTime? SecondPersonArrivalTime { get; set; }
        public int? SecondPersonArrivalId { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public int? Status { get; set; }
        public string? PlateNumber { get; set; }
        public bool? IsScheduled { get; set; }
        public int? CancelPersonId { get; set; }
        public string? CancelPersonFullname { get; set; }
        public string? CancelReason { get; set; }
    }
}