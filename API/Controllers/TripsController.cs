using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Trips;
using Application.Trips.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class TripsController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllTrips(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new ListTrips.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userId:int}/history")]
        public async Task<IActionResult> GetHistoryTrips(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new HistoryList.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userId:int}/upcoming")]
        public async Task<IActionResult> GetUpcomingTrips(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new UpcomingList.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // TODO: Search based on date, fromTime, toTime, departureId, destinationId
        // Biker, Admin
        [HttpGet("upcomingBiker")]
        public async Task<IActionResult> GetUpcomingTripsForBiker(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker && role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString()) + " " +
                                        ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new UpcomingListForBiker.Query {Page = page, Limit = limit, UserId = validationDto.UserRequestId},
                ct));
        }

        // TODO: Search Ke Now Trip

        // Keer, Biker, Admin
        [HttpGet("historyPair")]
        public async Task<IActionResult> GetHistoryPairTrips(int userOneId, int userTwoId, int page, int limit,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new HistoryPairList.Query {Page = page, Limit = limit, UserOneId = userOneId, UserTwoId = userTwoId},
                ct));
        }

        // Admin
        [HttpGet("{tripId:int}")]
        public async Task<IActionResult> GetTripDetail(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new DetailTrip.Query {TripId = tripId}, ct));
        }

        // Keer, Biker
        [HttpGet("{tripId:int}/details")]
        public async Task<IActionResult> GetTripDetailInfo(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        ConstantString.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new DetailTripInfo.Query {TripId = tripId, UserRequestId = validationDto.UserRequestId},
                ct));
        }

        // Keer
        [HttpPost]
        public async Task<IActionResult> CreateTrip(TripCreateDto tripCreateDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Keer.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, tripCreateDto.KeerId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

            return HandleResult(await Mediator.Send(new CreateTrip.Command {TripCreateDto = tripCreateDto}, ct));
        }

        // Biker
        [HttpPut("{tripId:int}")]
        public async Task<IActionResult> EditTripBiker(int tripId, int bikerId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

            return HandleResult(await Mediator.Send(new EditTripBiker.Command {TripId = tripId, BikerId = bikerId},
                ct));
        }

        // Biker
        [HttpPut("{tripId:int}/progressTime")]
        public async Task<IActionResult> EditTripProgressTime(int tripId, int bikerId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(ConstantString.NotSameUserId);

            return HandleResult(await Mediator.Send(new EditTripProcess.Command {TripId = tripId, BikerId = bikerId},
                ct));
        }

        // Keer, Biker, Admin
        [HttpPut("{tripId:int}/cancel")]
        public async Task<IActionResult> EditTripCancellation(int tripId, TripCancellationDto tripCancellationDto,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new EditTripCancellation.Command
                {
                    TripId = tripId,
                    UserId = validationDto.UserRequestId,
                    IsAdmin = validationDto.IsAdmin,
                    TripCancellationDto = tripCancellationDto
                }, ct));
        }
    }
}