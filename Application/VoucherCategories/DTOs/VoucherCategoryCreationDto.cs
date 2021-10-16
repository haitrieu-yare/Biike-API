using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Application.VoucherCategories.DTOs
{
    public class VoucherCategoryCreationDto
    {
        [Required] public string? CategoryName { get; init; }
    }
}