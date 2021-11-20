using System.ComponentModel.DataAnnotations;

namespace Application.Reports.DTOs
{
    public class ReportCreationDto
    {
        [Required] public int? UserOneId { get; set; }
        [Required] public int? UserTwoId { get; set; }
        [Required] public string? ReportReason { get; set; } 
    }
}