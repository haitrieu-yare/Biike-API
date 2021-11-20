using System.ComponentModel.DataAnnotations;

namespace Application.Sos.DTOs
{
    public class SosCreationDto
    {
        [Required] public string? SosName { get; set; }
        [Required] 
        [RegularExpression(@"^0([0-9]{9})$", ErrorMessage = "Invalid phone number.")]
        public string? SosPhone { get; set; }
    }
}