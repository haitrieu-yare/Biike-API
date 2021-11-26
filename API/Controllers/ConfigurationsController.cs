using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Configurations;
using Application.Configurations.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ConfigurationsController : BaseApiController
    {
        // Admin
        [HttpGet("all")]
        public async Task<IActionResult> GetAllConfigurations(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new ConfigurationList.Query(page, limit), ct));
        }
        
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetAllConfigurations(string configName, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ConfigurationDetails.Query(configName), ct));
        }

        // Admin 
        [HttpPost]
        public async Task<IActionResult> CreateConfiguration(ConfigurationCreationDto configurationCreationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new ConfigurationCreation.Command(configurationCreationDto, validationDto.UserRequestId), ct));
        }

        // Admin 
        [HttpPut]
        public async Task<IActionResult> EditConfiguration(ConfigurationDto configurationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new ConfigurationEdit.Command(configurationDto, validationDto.UserRequestId), ct));
        }
    }
}