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
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new ListTripTransactions.Query { Page = page, Limit = limit }, ct));
		}

		// Admin
		[HttpGet("{tripTransactionId:int}")]
		public async Task<IActionResult> GetSingleTripTransaction(int tripTransactionId, CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new DetailTripTransaction.Query { TripTransactionId = tripTransactionId }, ct));
		}

		// Admin
		[HttpGet("trips/{tripId:int}")]
		public async Task<IActionResult> GetTripTransactionsByTripId(int tripId, int page, int limit,
			CancellationToken ct)
		{
			int role = ControllerUtils.GetRole(HttpContext);

			if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

			if (role != (int) RoleStatus.Admin)
				return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) { StatusCode = 403 };

			return HandleResult(await Mediator.Send(
				new ListTripTransactionsByTripId.Query { TripId = tripId, Page = page, Limit = limit }, ct));
		}
	}
}