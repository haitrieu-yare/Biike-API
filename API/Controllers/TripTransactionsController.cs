using System.Threading;
using System.Threading.Tasks;
using Application.TripTransactions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class TripTransactionsController : BaseApiController
	{
		[Authorized(RoleStatus.Admin)]
		[HttpGet]
		public async Task<IActionResult> GetTripTransactions(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListTripTransactions.Query { Page = page, Limit = limit }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpGet("{tripTransactionId:int}")]
		public async Task<IActionResult> GetSingleTripTransaction(int tripTransactionId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransaction.Query { TripTransactionId = tripTransactionId }, ct));
		}

		[Authorized(RoleStatus.Admin)]
		[HttpGet("trips/{tripId:int}")]
		public async Task<IActionResult> GetTripTransaction(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransactionByTrip.Query { TripId = tripId }, ct));
		}
	}
}