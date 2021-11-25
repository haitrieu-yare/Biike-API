using System.ComponentModel.DataAnnotations;

namespace Application.Configurations.DTOs
{
    public class ConfigurationCreationDto
    {
        [Required] public string? ConfigurationName { get; set; }
        [Required] public string? ConfigurationValue { get; set; }
    }
}