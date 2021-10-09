using System;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class AuthController : BaseApiController
	{
		[HttpGet("testAuthentication")]
		public async Task<IActionResult> TestAuthentication()
		{
			var user = HttpContext.User;
			var isKeer = user.IsInRole("1");
			var isBiker = user.IsInRole("2");
			var isAdmin = user.IsInRole("3");

			HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeaders);
			FirebaseToken decodedToken =
				await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenHeaders.ToString().Split(" ")[1]);

			string uid = decodedToken.Uid;

			Console.WriteLine($"isKeer: {isKeer}");
			Console.WriteLine($"isBike: {isBiker}");
			Console.WriteLine($"isAdmin: {isAdmin}");
			Console.WriteLine($"uid: {uid}");

			return Ok();
		}
	}
}