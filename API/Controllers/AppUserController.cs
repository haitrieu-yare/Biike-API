using System.Threading;
using System.Threading.Tasks;
using Application.AppUsers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class AppUserController : BaseApiController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserProfile(int id, CancellationToken ct)
		{
			return HandleResult(await Mediator.Send(new Detail.Query { Id = id }, ct));
		}
	}
}