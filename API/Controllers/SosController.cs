using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Sos;
using Application.Sos.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class SosController : BaseApiController
    {
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetAllSos(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new SosList.Query(page, limit, validationDto.IsAdmin), ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{sosId:int}")]
        public async Task<IActionResult> GetSosBySosId(int sosId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new SosDetails.Query(sosId, validationDto.IsAdmin), ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateSos(SosCreationDto sosCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new SosCreation.Command(sosCreationDto), ct));
        }

        // Admin
        [HttpPut("{sosId:int}")]
        public async Task<IActionResult> EditSos(int sosId, SosDto newSosDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new SosEdit.Command(sosId, newSosDto), ct));
        }

        // Admin
        [HttpDelete("{sosId:int}")]
        public async Task<IActionResult> DeleteSos(int sosId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new SosDeletion.Command(sosId), ct));
        }
    }
}