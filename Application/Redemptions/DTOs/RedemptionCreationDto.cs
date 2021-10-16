using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Redemptions.DTOs
{
    public class RedemptionCreationDto
    {
        [Required] public int? UserId { get; init; }

        [Required] public int? VoucherId { get; init; }
    }
}