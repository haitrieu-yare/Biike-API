using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Images;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ImagesController : BaseApiController
    {
        // Keer, Biker, Admin
        [HttpPost]
        public async Task<IActionResult> UploadImage(CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new ImageUpload.Command(), ct));
        }
    }
}