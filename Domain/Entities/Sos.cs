using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Sos
    {
        public int SosId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string SosName { get; set; } = string.Empty;

        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string SosPhone { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = CurrentTime.GetCurrentTime();
    }
}