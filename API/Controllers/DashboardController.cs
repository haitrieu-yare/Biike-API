using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Dashboard;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class DashboardController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetDashboardInformation( CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new DashboardInformation.Query(), ct));
        }
    }
}