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

            return HandleResult(await Mediator.Send(new HistoryList.Query {Page = page, Limit = limit, UserId = userId},
                ct));
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
                new TripDetailsFull.Query {TripId = tripId, UserRequestId = validationDto.UserRequestId}, ct));
        }
        
        // Keer, Biker
        [HttpGet("{tripId:int}/waitingStatus")]
        public async Task<IActionResult> GetTripWaitingStatus(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new TripWaitingChecking.Query(tripId, validationDto.UserRequestId),
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

            return HandleResult(await Mediator.Send(new TripCreation.Command(tripCreationDto), ct));
        }

        // Keer
        [HttpPost("schedule")]
        // TODO: TripScheduleCreationDto không cần field isScheduled
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
                new TripScheduleCreation.Command(tripScheduleCreationDto), ct));
        }

        // Biker
        [HttpPut("{tripId:int}")]
        public async Task<IActionResult> EditTripBiker(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new TripBikerEdit.Command(tripId, validationDto.UserRequestId),
                ct));
        }
        
        // Keer, Biker
        [HttpPut("{tripId:int}/waitingTime")]
        public async Task<IActionResult> EditTripWaitingTime(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new TripWaitingEdit.Command(tripId, validationDto.UserRequestId),
                ct));
        }

        // Biker
        [HttpPut("{tripId:int}/startTime")]
        public async Task<IActionResult> EditTripStartTime(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new TripStartEdit.Command(tripId, validationDto.UserRequestId),
                ct));
        }

        // Biker
        [HttpPut("{tripId:int}/finishTime")]
        public async Task<IActionResult> EditTripFinishTime(int tripId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new TripFinishEdit.Command(tripId, validationDto.UserRequestId),
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