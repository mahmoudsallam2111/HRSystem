using HRSystem.Application.Features.Identity.Commands;
using HRSystem.Application.Features.Identity.Queries;
using HRSystem.Common.Requests.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers.Identity
{
    public class UserController : BaseController<UserController>
    {
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            var response = await Sender.Send(new UserRegistrationCommand { UserRegistrationRequest = userRegistrationRequest }); 

            if(response.IsSuccessful)
               return Ok(response);

            return BadRequest(response);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            var response = await Sender.Send(new UpdateUserCommand {  UpdateUserRequest = updateUserRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var response = await Sender.Send(new ChangePasswordCommand { ChangePasswordRequest = changePasswordRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatusRequest)
        {
            var response = await Sender.Send(new ChangeUserStatusCommand { ChangeUserStatusRequest = changeUserStatusRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await Sender.Send(new GetUserByIdQuery { UserId = id });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await Sender.Send(new GetAllUsersQuery {});

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

    }
}
