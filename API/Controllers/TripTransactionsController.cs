using System.Threading;
using System.Threading.Tasks;
using Application.TripTransactions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class TripTransactionsController : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetTripTransactions(int userId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new List.Query(), ct));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetSingleTripTransaction(int id, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}

		[HttpGet("trips/{tripId}")]
		public async Task<IActionResult> GetTripTransaction(int tripId, int role, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new DetailTrip.Query { TripId = tripId }, ct));
		}
	}
}