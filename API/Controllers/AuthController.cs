using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace API.Controllers
{
	[Authorize]
	public class AuthController : BaseApiController
	{
		[HttpGet("testAuthentication")]
		public async Task<IActionResult> TestAuthentication()
		{
			ClaimsPrincipal user = HttpContext.User;
			bool isKeer = user.IsInRole("1");
			bool isBiker = user.IsInRole("2");
			bool isAdmin = user.IsInRole("3");

			HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues tokenHeaders);
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