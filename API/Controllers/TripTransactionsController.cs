using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.TripTransactions;

namespace API.Controllers
{
	[Authorize]
	public class TripTransactionsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetTripTransactions(int page, int limit, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new ListTripTransactions.Query { Page = page, Limit = limit }, ct));
		}

		[HttpGet("{tripTransactionId}")]
		public async Task<IActionResult> GetSingleTripTransaction(int tripTransactionId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransaction.Query { TripTransactionId = tripTransactionId }, ct));
		}

		[HttpGet("trips/{tripId}")]
		public async Task<IActionResult> GetTripTransaction(int tripId, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransactionByTrip.Query { TripId = tripId }, ct));
		}
	}
}