using System.Threading;
using System.Threading.Tasks;
using Application.AppUsers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class UsersController : BaseApiController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}
	}
}