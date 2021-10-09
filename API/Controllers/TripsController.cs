using System;
using System.Threading;
using System.Threading.Tasks;
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
        [Authorized(RoleStatus.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllTrips(int page, int limit, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListTrips.Query {Page = page, Limit = limit}, ct));
        }

        [HttpGet("{userId:int}/history")]
        public async Task<IActionResult> GetHistoryTrips(int userId, int page, int limit, CancellationToken ct)
        {
            int role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized("Couldn't get user's role.");

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new HistoryList.Query {Page = page, Limit = limit, UserId = userId, Role = role}, ct));
        }

        [HttpGet("{userId:int}/upcoming")]
        public async Task<IActionResult> GetUpcomingTrips(int userId, int page, int limit, CancellationToken ct)
        {
            int role = ControllerUtils.GetRole(HttpContext);

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new UpcomingList.Query {Page = page, Limit = limit, UserId = userId, Role = role}, ct));
        }

        [Authorized(RoleStatus.Biker, RoleStatus.Admin)]
        [HttpGet("upcomingBiker")]
        public async Task<IActionResult> GetUpcomingTripsForBiker(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            return HandleResult(await Mediator.Send(
                new UpcomingListForBiker.Query
                {
                    Page = page,
                    Limit = limit,
                    UserId = validationDto.UserRequestId
                }, ct));
        }

        [HttpGet("historyPair")]
        public async Task<IActionResult> GetHistoryPairTrips(
            int userOneId, int userTwoId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new HistoryPairList.Query
                {
                    Page = page,
                    Limit = limit,
                    UserOneId = userOneId,
                    UserTwoId = userTwoId
                }, ct));
        }

        [Authorized(RoleStatus.Admin)]
        [HttpGet("{tripId:int}")]
        public async Task<IActionResult> GetTripDetail(int tripId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailTrip.Query {TripId = tripId}, ct));
        }

        [Authorized(RoleStatus.Keer, RoleStatus.Biker)]
        [HttpGet("{tripId:int}/details")]
        public async Task<IActionResult> GetTripDetailInfo(int tripId, CancellationToken ct)
        {
            int role = ControllerUtils.GetRole(HttpContext);

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            return HandleResult(await Mediator.Send(
                new DetailTripInfo.Query
                {
                    TripId = tripId,
                    Role = role,
                    UserRequestId = validationDto.UserRequestId
                }, ct));
        }

        [Authorized(RoleStatus.Keer)]
        [HttpPost]
        public async Task<IActionResult> CreateTrip(TripCreateDto tripCreateDto, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, tripCreateDto.KeerId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(new CreateTrip.Command {TripCreateDto = tripCreateDto}, ct));
        }

        [Authorized(RoleStatus.Biker)]
        [HttpPut("{tripId:int}")]
        public async Task<IActionResult> EditTripBiker(int tripId, int bikerId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new EditTripBiker.Command {TripId = tripId, BikerId = bikerId}, ct));
        }

        [Authorized(RoleStatus.Biker)]
        [HttpPut("{tripId:int}/progressTime")]
        public async Task<IActionResult> EditTripProgressTime(
            int tripId, int bikerId, string time, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, bikerId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new EditTripProcess.Command {TripId = tripId, BikerId = bikerId, Time = DateTime.Parse(time)}, ct));
        }

        [HttpPut("{tripId:int}/cancel")]
        public async Task<IActionResult> EditTripCancellation(int tripId, int userId,
            TripCancellationDto tripCancellationDto, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound)
                return BadRequest("Can't get userId who send the request.");

            if (!validationDto.IsAuthorized)
                return BadRequest("UserId of requester isn't the same with userId of trip.");

            return HandleResult(await Mediator.Send(
                new EditTripCancellation.Command
                {
                    TripId = tripId,
                    UserId = validationDto.UserRequestId,
                    TripCancellationDto = tripCancellationDto
                }, ct));
        }
    }
}