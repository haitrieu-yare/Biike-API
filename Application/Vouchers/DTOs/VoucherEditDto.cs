using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherEditDto
    {
        public int? VoucherCategoryId { get; set; }
        public string? VoucherName { get; set; }
        public string? Brand { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? AmountOfPoint { get; set; }
        public string? Description { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<int>? AddressIds { get; set; }
        public List<VoucherImageDto>? VoucherImages { get; set; }
    }
}