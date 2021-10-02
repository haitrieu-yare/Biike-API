using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FirebaseAdmin.Auth;

namespace API.Controllers
{
	[Authorize]
	public class AuthController : BaseApiController
	{
		[HttpGet("testAuthen")]
		public async Task<IActionResult> Verify()
		{
			var user = HttpContext.User;
			bool roleKeer = user.IsInRole("1");
			bool roleBiker = user.IsInRole("2");
			bool roleAdmin = user.IsInRole("3");

			HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeaders);
			FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
				.VerifyIdTokenAsync(tokenHeaders.ToString().Split(" ")[1]);

			string uid = decodedToken.Uid;

			return Ok();
		}
	}
}