using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.VoucherCategories;
using Application.VoucherCategories.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class VoucherCategoriesController : BaseApiController
    {
        // Keer, Biker, Admin
        [HttpGet]
        public async Task<IActionResult> GetVoucherCategories(int page, int limit, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new VoucherCategoryList.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{voucherCategoryId:int}")]
        public async Task<IActionResult> GetVoucherCategory(int voucherCategoryId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new VoucherCategoryDetails.Query {VoucherCategoryId = voucherCategoryId}, ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateVoucherCategory(
            VoucherCategoryCreationDto voucherCategoryCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherCategoryCreation.Command {VoucherCategoryCreationDto = voucherCategoryCreationDto}, ct));
        }

        // Admin
        [HttpPut("{voucherCategoryId:int}")]
        public async Task<IActionResult> EditVoucherCategory(
            int voucherCategoryId, VoucherCategoryDto newVoucherCategoryDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherCategoryEdit.Command
                {
                    VoucherCategoryId = voucherCategoryId,
                    NewVoucherCategoryDto = newVoucherCategoryDto
                }, ct));
        }

        // Admin
        [HttpDelete("{voucherCategoryId:int}")]
        public async Task<IActionResult> DeleteVoucherCategory(int voucherCategoryId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new VoucherCategoryDeletion.Command {VoucherCategoryId = voucherCategoryId}, ct));
        }
    }
}