using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Advertisements;
using Application.Advertisements.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdvertisementsController : BaseApiController
    {
        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement(AdvertisementCreationDto advertisementCreationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new AdvertisementCreation.Command(validationDto.UserRequestId, advertisementCreationDto), ct));
        }
    }
}