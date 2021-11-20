using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Reports;
using Application.Reports.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ReportsController : BaseApiController
    {
        // Admin
        [HttpGet("all")]
        public async Task<IActionResult> GetAllReports(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new ReportList.Query(page, limit), ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userOneId:int}")]
        public async Task<IActionResult> GetReportsByUserId(int userOneId, int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized)
                return new ObjectResult(Constant.DidNotHavePermissionToAccess) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new ReportListByUserId.Query(userOneId, page, limit), ct));
        }

        // Keer, Biker
        [HttpPost]
        public async Task<IActionResult> CreateReport(ReportCreationDto reportCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, reportCreationDto.UserOneId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(new ReportCreation.Command(reportCreationDto), ct));
        }

        // Keer, Biker
        [HttpPut("{reportId:int}/status/{status:int}")]
        public async Task<IActionResult> EditReportStatus(int reportId, int status, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new ReportStatusEdit.Command(reportId, status), ct));
        }
    }
}