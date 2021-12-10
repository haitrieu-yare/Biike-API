using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherCreationDto
    {
        [Required] public int? VoucherCategoryId { get; set; }

        [Required] public string? VoucherName { get; set; }

        [Required] public string? Brand { get; set; } 

        [Required] public DateTime? StartDate { get; set; }

        [Required] public DateTime? EndDate { get; set; }

        [Required] public int? AmountOfPoint { get; set; }

        [Required] public string? Description { get; set; }

        [Required] public string? TermsAndConditions { get; set; }
        public List<int> AddressIds { get; set; } = new();
        public List<string> VoucherImages { get; set; } = new();
    }
}