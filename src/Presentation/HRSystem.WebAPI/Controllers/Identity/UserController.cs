using HRSystem.Application.Features.Identity.Users.Commands;
using HRSystem.Application.Features.Identity.Users.Queries;
using HRSystem.Common.Authorization;
using HRSystem.Common.Requests.Identity;
using HRSystem.WebAPI.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers.Identity
{
    public class UserController : BaseController<UserController>
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            var response = await Sender.Send(new UserRegistrationCommand { UserRegistrationRequest = userRegistrationRequest }); 

            if(response.IsSuccessful)
               return Ok(response);

            return BadRequest(response);
        }


        [HttpPut]
        [MustHavePermession(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            var response = await Sender.Send(new UpdateUserCommand {  UpdateUserRequest = updateUserRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }


        [HttpPut("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var response = await Sender.Send(new ChangePasswordCommand { ChangePasswordRequest = changePasswordRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("change-status")]
        [MustHavePermession(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatusRequest)
        {
            var response = await Sender.Send(new ChangeUserStatusCommand { ChangeUserStatusRequest = changeUserStatusRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("{id}")]
        [MustHavePermession(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await Sender.Send(new GetUserByIdQuery { UserId = id });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        [HttpGet]
        [MustHavePermession(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetAll()
        {
            var response = await Sender.Send(new GetAllUsersQuery {});

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }


        [HttpGet("GetUserRoles/{Id}")]
        [MustHavePermession(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetUserRoles(string Id)
        {
            var response = await Sender.Send(new GetUserRolesQuery { UserId = Id });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }



        [HttpPut("UpdateUserRoles")]
        [MustHavePermession(AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRoleRequest updateUserRoleRequest)
        {
            var response = await Sender.Send(new UpdateUserRolesCommand {  UpdateUserRoleRequest = updateUserRoleRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

    }
}
