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

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(new ListVoucherCategory.Query {Page = page, Limit = limit}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{voucherCategoryId:int}")]
        public async Task<IActionResult> GetVoucherCategory(int voucherCategoryId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(ConstantString.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new DetailVoucherCategory.Query {VoucherCategoryId = voucherCategoryId}, ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateVoucherCategory(
            VoucherCategoryCreateDto voucherCategoryCreateDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new CreateVoucherCategory.Command {VoucherCategoryCreateDto = voucherCategoryCreateDto}, ct));
        }

        // Admin
        [HttpPut("{voucherCategoryId:int}")]
        public async Task<IActionResult> EditVoucherCategory(
            int voucherCategoryId, VoucherCategoryDto newVoucherCategoryDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new EditVoucherCategory.Command
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

            if (role == 0) return Unauthorized(ConstantString.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(ConstantString.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(
                new DeleteVoucherCategory.Command {VoucherCategoryId = voucherCategoryId}, ct));
        }
    }
}