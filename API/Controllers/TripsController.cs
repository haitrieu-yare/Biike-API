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

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new TripList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("history/users/{userId:int}")]
        public async Task<IActionResult> GetHistoryTrips(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new HistoryList.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("upcoming/users/{userId:int}")]
        public async Task<IActionResult> GetUpcomingTrips(int userId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new UpcomingList.Query {Page = page, Limit = limit, UserId = userId}, ct));
        }

        // Biker, Admin
        [HttpGet("newlyCreatedTrip")]
        public async Task<IActionResult> SearchNewlyCreatedTripList(int page, int limit, string? date, string? time,
            int departureId, int destinationId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker && role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new NewlyCreatedTripList.Query
                {
                    Page = page,
                    Limit = limit,
                    UserId = validationDto.UserRequestId,
                    Date = date,
                    Time = time,
                    DepartureId = departureId,
                    DestinationId = destinationId
                }, ct));
        }

        // TODO: Search Ke Now Trip

        // Keer, Biker, Admin
        [HttpGet("historyPair")]
        public async Task<IActionResult> GetHistoryPairTrips(int userOneId, int userTwoId, int page, int limit,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(
                new HistoryPairList.Query {Page = page, Limit = limit, UserOneId = userOneId, UserTwoId = userTwoId},
                ct));
        }

        // Admin
        [HttpGet("{tripId:int}")]
        public async Task<IActionResult> GetTripDetail(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new TripDetails.Query {TripId = tripId}, ct));
        }

        // Keer, Biker
        [HttpGet("{tripId:int}/details")]
        public async Task<IActionResult> GetTripDetailInfo(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new TripDetailsFull.Query {TripId = tripId, UserRequestId = validationDto.UserRequestId},
                ct));
        }

        // Keer
        [HttpPost]
        public async Task<IActionResult> CreateTrip(TripCreationDto tripCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, tripCreationDto.KeerId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(new TripCreation.Command {TripCreationDto = tripCreationDto}, ct));
        }
        
        // Keer
        [HttpPost("schedule")]
        public async Task<IActionResult> CreateTripSchedule(TripScheduleCreationDto tripScheduleCreationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, tripScheduleCreationDto.KeerId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(
                new TripScheduleCreation.Command {TripScheduleCreationDto = tripScheduleCreationDto}, ct));
        }

        // Biker
        [HttpPut("{tripId:int}")]
        public async Task<IActionResult> EditTripBiker(int tripId, int bikerId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(new TripBikerEdit.Command {TripId = tripId, BikerId = bikerId},
                ct));
        }

        // Biker
        [HttpPut("{tripId:int}/progressTime")]
        public async Task<IActionResult> EditTripProgressTime(int tripId, int bikerId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(new TripProcessEdit.Command {TripId = tripId, BikerId = bikerId},
                ct));
        }

        // Keer, Biker, Admin
        [HttpPut("{tripId:int}/cancel")]
        public async Task<IActionResult> EditTripCancellation(int tripId, TripCancellationDto tripCancellationDto,
            CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new TripCancellationEdit.Command
                {
                    TripId = tripId,
                    UserId = validationDto.UserRequestId,
                    IsAdmin = validationDto.IsAdmin,
                    TripCancellationDto = tripCancellationDto
                }, ct));
        }
    }
}