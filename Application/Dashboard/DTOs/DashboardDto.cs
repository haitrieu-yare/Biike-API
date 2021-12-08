using System.Collections.Generic;

namespace Application.Dashboard.DTOs
{
    public class DashboardDto
    {
        public int? TotalUser { get; set; }
        public int? TotalNewUser { get; set; }
        public int? TotalTrip { get; set; }
        public int? TotalRedemption { get; set; }
        public int? TotalPointUsedForVoucher { get; set; }
        public int? TotalAdsClickCount { get; set; }
        public double? TotalKmSaved { get; set; }
        public double? TotalFuelSaved { get; set; }
        public List<StationPercentageDto> StationPercentage { get; set; } = new();
        public List<TripStatusPercentageDto> TripStatusPercentage { get; set; } = new();
    }
}