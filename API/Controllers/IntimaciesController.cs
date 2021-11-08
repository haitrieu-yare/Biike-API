using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Intimacies;
using Application.Intimacies.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class IntimaciesController : BaseApiController
    {
        // Admin
        [HttpGet("all")]
        public async Task<IActionResult> GetAllIntimacies(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new IntimacyList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userOneId:int}")]
        public async Task<IActionResult> GetIntimaciesByUserId(int userOneId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized)
                return new ObjectResult(Constant.DidNotHavePermissionToAccess) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new IntimacyListByUserId.Query {Page = page, Limit = limit, UserOneId = userOneId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> CheckIntimacy(int userOneId, int userTwoId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized)
                return new ObjectResult(Constant.DidNotHavePermissionToAccess) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new IntimacyPair.Query(userOneId, userTwoId), ct));
        }

        // Keer, Biker
        [HttpPost]
        public async Task<IActionResult> CreateIntimacy(IntimacyModificationDto intimacyModificationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, intimacyModificationDto.UserOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new IntimacyCreation.Command {IntimacyModificationDto = intimacyModificationDto}, ct));
        }

        // Keer, Biker
        [HttpPut]
        public async Task<IActionResult> EditIntimacies(IntimacyModificationDto intimacyModificationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, intimacyModificationDto.UserOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new IntimacyEdit.Command {IntimacyModificationDto = intimacyModificationDto}, ct));
        }
    }
}