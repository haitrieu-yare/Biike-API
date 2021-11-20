using System.Collections.Generic;
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
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetAdvertisements(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new AdvertisementList.Query(page, limit), ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{advertisementId:int}")]
        public async Task<IActionResult> GetAdvertisement(int advertisementId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new AdvertisementDetails.Query(advertisementId), ct));
        }

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

        // Admin
        [HttpPut("{advertisementId:int}")]
        public async Task<IActionResult> EditAdvertisement(int advertisementId, AdvertisementEditDto newAdvertisement,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AdvertisementEdit.Command(advertisementId, newAdvertisement),
                ct));
        }
        
        // Keer, Biker
        [HttpPut("{advertisementId:int}/clickCount")]
        public async Task<IActionResult> EditClickCountAdvertisement(int advertisementId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AdvertisementClickCountEdit.Command(advertisementId),
                ct));
        }
        
        // Admin
        [HttpDelete("{advertisementId:int}")]
        public async Task<IActionResult> DeleteAdvertisement(int advertisementId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AdvertisementDeletion.Command(advertisementId), ct));
        }

        // Admin
        // This endpoint purpose is add new images for existing advertisement
        // in case admin want to add more images for this advertisement
        // This endpoint does not upload image to Firebase
        // It only create image records for advertisement
        [HttpPost("{advertisementId:int}/images")]
        public async Task<IActionResult> CreateAdvertisementImage(int advertisementId,
            [FromBody] List<string> advertisementImages, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new AdvertisementImageCreation.Command(advertisementId, advertisementImages), ct));
        }

        // Admin
        // This endpoint purpose is remove images for existing advertisement
        // in case admin want to remove images for this advertisement
        // This endpoint does not remove image from Firebase
        // It only remove image records for advertisement
        [HttpDelete("images")]
        public async Task<IActionResult> DeleteAdvertisementImage(
            AdvertisementImageDeletionDto advertisementImageDeletionDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new AdvertisementImageDeletion.Command(advertisementImageDeletionDto), ct));
        }
    }
}