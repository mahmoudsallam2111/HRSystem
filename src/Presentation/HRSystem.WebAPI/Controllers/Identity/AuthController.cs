using HRSystem.Application.Features.Identity.Token.Queries;
using HRSystem.Common.Requests.Identity;
using HRSystem.Common.Responses.Identity;
using HRSystem.Common.Responses.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController<AuthController>
    {


        [HttpPost("get-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper<TokenResponse>>> GetToken([FromBody] TokenRequest tokenRequest)
        {
            var tokenResponse = await Sender.Send(new GetTokenQuery { tokenRequest = tokenRequest });
            if (tokenResponse.IsSuccessful)
                return Ok(tokenResponse);
            else return BadRequest(tokenResponse);
        }

        [HttpPost("get-refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseWrapper<TokenResponse>>> GetRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var tokenResponse = await Sender.Send(new GetRefreshTokenQuery { refreshTokenRequest = refreshTokenRequest });
            if (tokenResponse.IsSuccessful)
                return Ok(tokenResponse);
            else return BadRequest(tokenResponse);
        }



    }
}
