using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.TripTransactions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class TripTransactionsController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetTripTransactions(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new TripTransactionList.Query {Page = page, Limit = limit}, ct));
        }

        // Admin
        [HttpGet("{tripTransactionId:int}")]
        public async Task<IActionResult> GetSingleTripTransaction(int tripTransactionId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new TripTransactionDetails.Query {TripTransactionId = tripTransactionId}, ct));
        }

        // Admin
        [HttpGet("trips/{tripId:int}")]
        public async Task<IActionResult> GetTripTransactionsByTripId(int tripId, int page, int limit,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new TripTransactionListByTripId.Query {TripId = tripId, Page = page, Limit = limit}, ct));
        }
    }
}