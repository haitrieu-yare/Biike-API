using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.Intimacies.DTOs
{
    public class IntimacyModificationDto
    {
        [Required] public int? UserOneId { get; init; }

        [Required] public int? UserTwoId { get; init; }
    }
}