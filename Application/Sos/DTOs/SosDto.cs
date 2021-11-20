using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Sos.DTOs
{
    public class SosDto
    {
        public int? SosId { get; set; }
        public string? SosName { get; set; }
        
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string? SosPhone { get; set; }
        public DateTime? CreatedDate { get; set; } 
    }
}