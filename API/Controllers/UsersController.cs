using System;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Users;
using Application.Users.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        // Admin
        [HttpGet]
        public async Task<IActionResult> GetAllUser(int page, int limit, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new UserList.Query {Page = page, Limit = limit}, ct));
        }
        
        // Keer, Biker, Admin
        [HttpGet("topBiker")]
        public async Task<IActionResult> GetTopBiker(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new TopBiker.Query(), ct));
        }

        // Keer, Biker
        [HttpGet("{userId:int}/profile")]
        public async Task<IActionResult> GetUserSelfProfile(int userId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.DidNotHavePermissionToAccess);

            return HandleResult(await Mediator.Send(new UserSelfDetails.Query {UserId = userId}, ct));
        }

        // Keer, Biker, Admin
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserProfile(int userId, CancellationToken ct)
        {
            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new UserDetails.Query {IsAdmin = validationDto.IsAdmin, UserId = userId}, ct));
        }

        [AllowAnonymous]
        [HttpPost("checkExist")]
        public async Task<IActionResult> CheckExistUser(UserExistenceDto userExistenceDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(
                new UserExistence.Command {UserExistenceDto = userExistenceDto}, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(UserCreationDto userCreationDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new UserCreation.Command(userCreationDto), ct));
        }

        // TODO: When a user can modify account activation?
        [AllowAnonymous]
        [HttpPut("{userId:int}/activation")]
        public async Task<IActionResult> ModifyAccountActivation(
            int userId, UserActivationDto userActivationDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(
                new AccountActivationEdit.Command {UserId = userId, UserActivationDto = userActivationDto}, ct));
        }

        [AllowAnonymous]
        [HttpGet("{userId:int}/activation")]
        public async Task<IActionResult> CheckAccountActivation(int userId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(
                new AccountActivation.Query {UserId = userId}, ct));
        }

        // Keer, Biker
        [HttpPut("{userId:int}/profile")]
        public async Task<IActionResult> EditUserProfile(int userId, UserProfileEditDto userProfileEditDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            return HandleResult(await Mediator.Send(new UserProfileEdit.Command(userId, userProfileEditDto), ct));
        }

        // Keer, Biker
        [HttpPut("role")]
        public async Task<IActionResult> EditUserRole(int startupRole, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new UserRoleEdit.Command {UserId = validationDto.UserRequestId, StartupRole = startupRole}, ct));
        }

        // Admin
        [HttpPut("{userId:int}")]
        public async Task<IActionResult> EditUserStatus(int userId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new UserStatusEdit.Command {UserId = userId}, ct));
        }
        
        // Keer, Biker
        [HttpPost("addresses")]
        public async Task<IActionResult> CreateUserAddress(UserAddressCreationDto userAddressCreationDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(
                await Mediator.Send(new UserAddressCreation.Command(validationDto.UserRequestId, userAddressCreationDto),
                    ct));
        }
        
        // Keer, Biker
        [HttpPut("addresses/{addressId:int}")]
        public async Task<IActionResult> EditUserAddress(int addressId, UserAddressDto userAddressDto,
            CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            return HandleResult(await Mediator.Send(
                new UserAddressEdit.Command(validationDto.UserRequestId, addressId, userAddressDto), ct));
        }

        // Keer, Biker
        [HttpPut("{userId:int}/login-device")]
        public async Task<IActionResult> EditUserLoginDevice(int userId,
            UserLoginDeviceDto userLoginDeviceDto, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Keer && role != (int) RoleStatus.Biker)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Keer.ToString()) + " " +
                                        Constant.OnlyRole(RoleStatus.Biker.ToString())) {StatusCode = 403};

            ValidationDto validationDto = ControllerUtils.Validate(HttpContext, userId);

            if (!validationDto.IsUserFound) return BadRequest(Constant.CouldNotGetIdOfUserSentRequest);

            if (!validationDto.IsAuthorized) return BadRequest(Constant.NotSameUserId);

            var userAuthTimeClaim = HttpContext.User.FindFirst(c => c.Type.Equals("auth_time"));
            var authTimeString = userAuthTimeClaim?.Value;

            if (string.IsNullOrEmpty(authTimeString))
                return BadRequest("Can't get authentication time of the who send the request.");

            var authTime = int.Parse(authTimeString);
            var beginningTime = DateTime.UnixEpoch;
            var currentTimeUtc7 = beginningTime.AddSeconds(authTime).AddHours(7);

            userLoginDeviceDto.LastTimeLogin = currentTimeUtc7;

            return HandleResult(await Mediator.Send(
                new LoginDeviceEdit.Command {UserId = userId, UserLoginDeviceDto = userLoginDeviceDto}, ct));
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin(UserLoginDto userLoginDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new UserLoginRequest.Command {UserLoginDto = userLoginDto}, ct));
        }

        // Admin
        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new UserDeletion.Command {UserId = userId}, ct));
        }

        // Admin
        [HttpDelete("deleteFirebase")]
        public async Task<IActionResult> DeleteAllFirebaseUsers(CancellationToken ct)
        {
            var role = ControllerUtils.GetRole(HttpContext);

            if (role == 0) return Unauthorized(Constant.CouldNotGetUserRole);

            if (role != (int) RoleStatus.Admin)
                return new ObjectResult(Constant.OnlyRole(RoleStatus.Admin.ToString())) {StatusCode = 403};

            return HandleResult(await Mediator.Send(new FireBaseUserDeletion.Command(), ct));
        }
    }
}