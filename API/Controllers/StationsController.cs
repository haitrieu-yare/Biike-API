using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Stations;
using Application.Stations.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class StationsController : BaseApiController
    {
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetAllStations(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new StationList.Query {Page = page, Limit = limit, IsAdmin = validationDto.IsAdmin}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("relatedStations")]
        public async Task<IActionResult> GetAllStationsByStationId(int page, int limit, int departureId,
            int destinationId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new StationListByStationId.Query
                {
                    Page = page,
                    Limit = limit,
                    IsAdmin = validationDto.IsAdmin,
                    DepartureId = departureId,
                    DestinationId = destinationId
                }, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{stationId:int}")]
        public async Task<IActionResult> GetStationByStationId(int stationId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new StationDetails.Query {StationId = stationId, IsAdmin = validationDto.IsAdmin}, ct));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStation(StationCreationDto stationCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new StationCreation.Command {StationCreationDto = stationCreationDto}, ct));
        }

        // Admin
        [HttpPut("{stationId:int}")]
        public async Task<IActionResult> EditStation(int stationId, StationDto newStationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new StationEdit.Command {StationId = stationId, NewStationDto = newStationDto}, ct));
        }

        // Admin
        [HttpDelete("{stationId:int}")]
        public async Task<IActionResult> DeleteStation(int stationId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new StationDeletion.Command {StationId = stationId}, ct));
        }
    }
}