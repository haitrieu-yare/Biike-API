namespace Application.Users.DTOs
{
    public class UserAchievementDto
    {
        public int? UserId { get; set; }
        public int? TotalKeerTrip { get; set; }
        public int? TotalBikerTrip { get; set; }
        public double? TotalKmSaved { get; set; }
        public double? TotalFuelSaved { get; set; }
    }
}