using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Addresses;
using Application.Addresses.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class AddressesController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> ListAllAddresses(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AddressList.Query(page, limit), ct));
        }
        
        // Admin
        [HttpGet("{addressId:int}")]
        public async Task<IActionResult> ListAllAddresses(int addressId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AddressDetails.Query(addressId), ct));
        }

        // Admin
        [HttpPost]
        public async Task<IActionResult> CreateAddress(AddressCreationDto addressCreationDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AddressCreation.Command(addressCreationDto), ct));
        }

        // Admin
        [HttpPut]
        public async Task<IActionResult> EditAddress(AddressDto addressDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AddressEdit.Command(addressDto), ct));
        }

        // Admin
        [HttpDelete("{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int addressId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new AddressDeletion.Command(addressId), ct));
        }
    }
}