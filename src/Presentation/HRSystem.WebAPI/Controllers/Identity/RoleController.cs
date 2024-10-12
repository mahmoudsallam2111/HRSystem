using HRSystem.Application.Features.Identity.Roles.Commands;
using HRSystem.Application.Features.Identity.Roles.Queries;
using HRSystem.Application.Features.Identity.Users.Commands;
using HRSystem.Common.Authorization;
using HRSystem.Common.Requests.Identity;
using HRSystem.WebAPI.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers.Identity
{
    public class RoleController : BaseController<RoleController>
    {
        [HttpPost]
        [MustHavePermession(AppFeature.Roles, AppAction.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
        {
            var response = await Sender.Send(new CreateRoleCommand { CreateRoleRequest = createRoleRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }


        [HttpGet]
        [MustHavePermession(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetAll()
        {
            var response = await Sender.Send(new GetRolesQuery { });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }



        [HttpGet("{Id}")]
        [MustHavePermession(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetById(string Id)
        {
            var response = await Sender.Send(new GetRoleByIdQuery {  RoleId  = Id});

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }


        [HttpGet("get-role-permessions/{Id}")]
        [MustHavePermession(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRolePermessions(string Id)
        {
            var response = await Sender.Send(new GetRolePermessionsQuery { RoleId = Id });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        [HttpPut("update-role-permessions")]
        [MustHavePermession(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> UpdateRolePermessions(UpdateRoleClaimsRequest updateRoleClaimsRequest)
        {
            var response = await Sender.Send(new UpdateRolePermessionCommand { updateRoleClaimsRequest = updateRoleClaimsRequest});

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }


        [HttpPut]
        [MustHavePermession(AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateRole(UpdateRoleRequest updateRoleRequest)
        {
            var response = await Sender.Send(new UpdateRoleCommand { UpdateRoleRequest = updateRoleRequest });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpDelete("{Id}")]
        [MustHavePermession(AppFeature.Roles, AppAction.Delete)]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var response = await Sender.Send(new DeleteRoleCommand {RoleId  = Id});

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
