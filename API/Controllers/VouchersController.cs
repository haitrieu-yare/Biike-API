using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Vouchers;
using Application.Vouchers.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class VouchersController : BaseApiController
    {
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetVouchers(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new VoucherList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{voucherId:int}")]
        public async Task<IActionResult> GetVoucher(int voucherId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new VoucherDetails.Query {VoucherId = voucherId}, ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateVoucher(VoucherCreationDto voucherCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherCreation.Command(voucherCreationDto), ct));
        }

        // Admin
        [HttpPut("{voucherId:int}")]
        public async Task<IActionResult> EditVoucher(int voucherId, VoucherEditDto newVoucher, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherEdit.Command {VoucherId = voucherId, NewVoucher = newVoucher}, ct));
        }

        // Admin
        [HttpPost("{voucherId:int}/images")]
        public async Task<IActionResult> CreateVoucherImage(int voucherId,[FromBody] List<string> 
            voucherImages,CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherImageCreation.Command(voucherId, voucherImages), ct));
        }

        // Admin
        [HttpDelete("images")]
        public async Task<IActionResult> DeleteVoucherImage(VoucherImageDeletionDto voucherImageDeletionDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherImageDeletion.Command(voucherImageDeletionDto), ct));
        }
    }
}