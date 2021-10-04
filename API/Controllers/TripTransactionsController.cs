using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.TripTransactions;

namespace API.Controllers
{
	public class TripTransactionsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetTripTransactions(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new ListTripTransactions.Query(), ct));
		}

		[HttpGet("{tripTransactionId}")]
		public async Task<IActionResult> GetSingleTripTransaction(int tripTransactionId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransaction.Query { TripTransactionId = tripTransactionId }, ct));
		}

		[HttpGet("trips/{tripId}")]
		public async Task<IActionResult> GetTripTransaction(int tripId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(
				new DetailTripTransactionByTrip.Query { TripId = tripId }, ct));
		}
	}
}