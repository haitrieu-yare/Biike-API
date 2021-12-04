using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.BikeAvailabilities;
using Application.BikeAvailabilities.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class BikeAvailabilitiesController : BaseApiController
    {
        // Biker 
        [HttpGet]
        public async Task<IActionResult> GetAllBikeAvailabilitiesByUseId(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new BikeAvailabilityListByUserId.Query(page, limit, validationDto.UserRequestId), ct));
        }

        // Biker 
        [HttpGet("{bikeAvailabilityId:int}")]
        public async Task<IActionResult> GetBikeAvailability(int bikeAvailabilityId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new BikeAvailabilityDetails.Query(bikeAvailabilityId, validationDto.UserRequestId), ct));
        }

        // Biker 
        [HttpPost]
        public async Task<IActionResult> CreateBikeAvailability(
            BikeAvailabilityModificationDto bikeAvailabilityModificationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new BikeAvailabilityCreation.Command(bikeAvailabilityModificationDto, validationDto.UserRequestId),
                ct));
        }

        // Biker 
        [HttpPut("{bikeAvailabilityId:int}")]
        public async Task<IActionResult> UpdateBikeAvailability(
            BikeAvailabilityModificationDto bikeAvailabilityModificationDto, int bikeAvailabilityId,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new BikeAvailabilityEdit.Command(bikeAvailabilityModificationDto, validationDto.UserRequestId,
                    bikeAvailabilityId), ct));
        }

        // Biker 
        [HttpDelete("{bikeAvailabilityId:int}")]
        public async Task<IActionResult> CreateBikeAvailability(int bikeAvailabilityId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new BikeAvailabilityDeletion.Command(bikeAvailabilityId, validationDto.UserRequestId), ct));
        }
    }
}