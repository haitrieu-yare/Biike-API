using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.VoucherCodes;
using Application.VoucherCodes.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class VoucherCodesController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetVoucherCodes(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeList.Query(page, limit), ct));
        }

        // Admin
        [HttpGet("vouchers/{voucherId:int}")]
        public async Task<IActionResult> GetVoucherCodesByVoucherId(int page, int limit, int voucherId,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeListByVoucherId.Query(page, limit, voucherId), ct));
        }

        // Admin
        [HttpGet("{voucherCodeId:int}")]
        public async Task<IActionResult> GetVoucherCodeDetails(int voucherCodeId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeDetails.Query(voucherCodeId), ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateVoucherCodes(int voucherId, [FromBody] List<string> voucherCodes,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeCreation.Command(voucherId, voucherCodes), ct));
        }

        // Admin
        [HttpPut("{voucherCodeId:int}")]
        public async Task<IActionResult> EditVoucherCode(int voucherCodeId, VoucherCodeDto voucherCodeDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeEdit.Command(voucherCodeId, voucherCodeDto), ct));
        }
        
        // Admin
        [HttpDelete]
        public async Task<IActionResult> DeleteVoucherCode([FromBody] List<int> voucherCodeIds, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new VoucherCodeDeletion.Command(voucherCodeIds), ct));
        }
    }
}