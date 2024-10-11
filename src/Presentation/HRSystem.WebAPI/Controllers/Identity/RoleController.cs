using HRSystem.Application.Features.Identity.Roles.Commands;
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
    }
}
