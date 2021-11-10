using System;
using System.Collections.Generic;
using Application.Addresses.DTOs;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

namespace Application.Vouchers.DTOs
{
    public class VoucherDto
    {
        public int? VoucherId { get; set; }
        public int? VoucherCategoryId { get; set; }
        public string? VoucherCategoryName { get; set; }
        public string? VoucherName { get; set; }
        public string? Brand { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Quantity { get; set; }
        public int? Remaining { get; set; }
        public int? AmountOfPoint { get; set; }
        public string? Description { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<AddressDto>? VoucherAddresses { get; set; }
        public List<VoucherImageDto>? VoucherImages { get; set; }
    }
}