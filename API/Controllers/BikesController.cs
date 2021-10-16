using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Bikes;
using Application.Bikes.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class BikesController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllBikes(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new BikeList.Query {Page = page, Limit = limit}, ct));
        }

        // Admin
        [HttpGet("{bikeId:int}")]
        public async Task<IActionResult> GetBikeByBikeId(int bikeId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new BikeDetailsByBikeId.Query {BikeId = bikeId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetBikeByUserId(int userId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized)
                return new ObjectResult(ConstantString.DidNotHavePermissionToAccess) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new BikeDetailsByUserId.Query {IsAdmin = validationDto.IsAdmin, UserId = validationDto.UserRequestId},
                ct));
        }

        // Keer
        [HttpPost]
        public async Task<IActionResult> CreateBike(BikeCreationDto bikeCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikeCreationDto.UserId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

            return HandleResult(await Mediator.Send(new BikeCreation.Command {BikeCreationDto = bikeCreationDto}, ct));
        }

        // Biker
        [HttpPost("bikeReplacement")]
        public async Task<IActionResult> ReplaceBike(BikeCreationDto bikeCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikeCreationDto.UserId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

            return HandleResult(await Mediator.Send(new BikeReplacement.Command {BikeCreationDto = bikeCreationDto}, ct));
        }

        // Biker
        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> DeleteBike(int userId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized)
                return new ObjectResult(ConstantString.DidNotHavePermissionToMakeRequest) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new BikeDeletion.Command {UserId = userId}, ct));
        }
    }
}