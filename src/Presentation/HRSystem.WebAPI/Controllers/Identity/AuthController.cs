using HRSystem.Application.Features.Identity.Queries;
using HRSystem.Application.Services.Identity;
using HRSystem.Common.Requests;
using HRSystem.Common.Responses;
using HRSystem.Common.Responses.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<ResponseWrapper<TokenResponse>>> GetRefreshToken([FromBody] RefreshTokenRequest  refreshTokenRequest)
        {
            var tokenResponse = await Sender.Send(new GetRefreshTokenQuery { refreshTokenRequest = refreshTokenRequest });
            if (tokenResponse.IsSuccessful)
                return Ok(tokenResponse);
            else return BadRequest(tokenResponse);
        }



    }
}
