using System.ComponentModel.DataAnnotations;

namespace Application.Bikes.DTOs
{
    public class BikeVerificationDto
    {
        [Required] public bool? VerificationResult { get; set; }
        public string? FailedVerificationReason { get; set; }
    }
}