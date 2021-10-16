using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Images.DTOs
{
    public class ImageUploadRequestDto
    {
        [Required] public IFormFile? Image { get; set; }
    }
}